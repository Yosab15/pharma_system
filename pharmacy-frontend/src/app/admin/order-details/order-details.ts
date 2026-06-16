import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

interface OrderItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  total: number;
}

interface OrderDto {
  id: number;
  customerName: string;
  responsibleName: string;
  phoneNumber: string;
  address: string;
  city: number;
  notes: string;
  supplyOrderImageUrl: string | null;
  latitude: number | null;
  longitude: number | null;
  paymentType: string;
  freeUnitsReceived: number;
  totalPrice: number;
  discount: number;
  finalPrice: number;
  orderDate: string;
  status: number;
  items: OrderItem[];
}

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order-details.html',
  styleUrl: './order-details.css'
})
export class OrderDetails implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  order = signal<OrderDto | null>(null);
  loading = signal(true);
  discountInput = signal(0);
  updatingDiscount = signal(false);

  readonly statusMap: Record<number, string> = {
    0: 'Pending', 1: 'Contacted', 2: 'Delivered', 3: 'Cancelled'
  };

  readonly statusClass: Record<number, string> = {
    0: 'status-pending', 1: 'status-contacted',
    2: 'status-delivered', 3: 'status-cancelled'
  };

  readonly cityMap: Record<number, string> = {
    0: 'Cairo', 1: 'Alexandria', 2: 'Giza', 3: 'Shubra El Kheima',
    4: 'Port Said', 5: 'Suez', 6: 'Luxor', 7: 'Mansoura',
    8: 'El Mahalla El Kubra', 9: 'Tanta', 10: 'Asyut', 11: 'Ismailia',
    12: 'Faiyum', 13: 'Zagazig', 14: 'Aswan', 15: 'Damietta',
    16: 'Damanhur', 17: 'Minya', 18: 'Beni Suef', 19: 'Qena',
    20: 'Sohag', 21: 'Hurghada', 22: 'Sixth of October', 23: 'Shibin El Kom',
    24: 'Banha', 25: 'Kafr El Sheikh', 26: 'Arish', 27: 'Mallawi',
    28: 'Mit Ghamr', 29: 'Obour'
  };

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadOrder(+id);
  }

  loadOrder(id: number) {
    this.http.get<OrderDto>(`https://localhost:44313/api/Order/${id}`)
      .subscribe({
        next: (res) => {
          this.order.set(res);
          this.discountInput.set(res.discount || 0);
          this.loading.set(false);
        },
        error: () => {
          this.toastr.error('Failed to load order details!');
          this.loading.set(false);
        }
      });
  }

  applyDiscount() {
  const o = this.order();
  if (!o) return;

  this.updatingDiscount.set(true);
  this.http.put(
    `https://localhost:44313/api/Order/${o.id}/discount`,
    { discount: this.discountInput() },
    { responseType: 'text' }
  ).subscribe({
    next: () => {
      this.toastr.success('Discount applied successfully!');
      this.order.update(order => order ? {
        ...order,
        discount: this.discountInput(),
        finalPrice: order.totalPrice - (order.totalPrice * this.discountInput() / 100)
      } : order);
      this.updatingDiscount.set(false);
    },
    error: () => {
      this.toastr.error('Failed to apply discount!');
      this.updatingDiscount.set(false);
    }
  });
}

  goBack() {
    this.router.navigate(['/admin/orders']);
  }

  openImage(url: string) {
    window.open('https://localhost:44313' + url, '_blank');
  }
}
