import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OrderService, Order } from '../../core/services/order';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class Orders {
  private orderService = inject(OrderService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  // Signals
  orders = signal<any[]>([]);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(10);
  statusFilter = signal('');
  searchFilter = signal('');
  cityFilter = signal('');
  loading = signal(false);

  // Computed
  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  totalOrders = computed(() => this.totalCount());

  readonly statuses = ['', 'Pending', 'Contacted', 'Delivered', 'Cancelled'];

  readonly cities = [
    { value: '', label: 'All Cities' },
    { value: '0', label: 'Cairo' },
    { value: '1', label: 'Alexandria' },
    { value: '2', label: 'Giza' },
    { value: '3', label: 'Shubra El Kheima' },
    { value: '4', label: 'Port Said' },
    { value: '5', label: 'Suez' },
    { value: '6', label: 'Luxor' },
    { value: '7', label: 'Mansoura' },
    { value: '8', label: 'El Mahalla El Kubra' },
    { value: '9', label: 'Tanta' },
    { value: '10', label: 'Asyut' },
    { value: '11', label: 'Ismailia' },
    { value: '12', label: 'Faiyum' },
    { value: '13', label: 'Zagazig' },
    { value: '14', label: 'Aswan' },
    { value: '15', label: 'Damietta' },
    { value: '16', label: 'Damanhur' },
    { value: '17', label: 'Minya' },
    { value: '18', label: 'Beni Suef' },
    { value: '19', label: 'Qena' },
    { value: '20', label: 'Sohag' },
    { value: '21', label: 'Hurghada' },
    { value: '22', label: 'Sixth of October' },
    { value: '23', label: 'Shibin El Kom' },
    { value: '24', label: 'Banha' },
    { value: '25', label: 'Kafr El Sheikh' },
    { value: '26', label: 'Arish' },
    { value: '27', label: 'Mallawi' },
    { value: '28', label: 'Mit Ghamr' },
    { value: '29', label: 'Obour' },
  ];

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

  readonly statusMap: Record<number, string> = {
    0: 'Pending', 1: 'Contacted', 2: 'Delivered', 3: 'Cancelled'
  };

  readonly statusIndexMap: Record<string, number> = {
    'Pending': 0, 'Contacted': 1, 'Delivered': 2, 'Cancelled': 3
  };

  statusLabel(status: any): string {
    return this.statusMap[status] ?? status;
  }

  cityLabel(city: any): string {
    return this.cityMap[city] ?? city;
  }

  constructor() {
    effect(() => {
      const page = this.pageNumber();
      const status = this.statusFilter();
      const search = this.searchFilter();
      const city = this.cityFilter();
      this.loadOrders(page, status, search, city);
    });
  }

  loadOrders(page: number, status: string, search: string, city: string) {
    this.loading.set(true);
    this.orderService.getOrders(page, this.pageSize(), status, search, city)
      .subscribe({
        next: (res) => {
          this.orders.set(res.data);
          this.totalCount.set(res.totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.toastr.error('Failed to load orders!');
          this.loading.set(false);
        }
      });
  }

  onStatusFilter(status: string) {
    this.statusFilter.set(status);
    this.pageNumber.set(1);
  }

  onSearch(search: string) {
    this.searchFilter.set(search);
    this.pageNumber.set(1);
  }

  onCityFilter(city: string) {
    this.cityFilter.set(city);
    this.pageNumber.set(1);
  }

  nextPage() {
    if (this.pageNumber() < this.totalPages())
      this.pageNumber.update(p => p + 1);
  }

  prevPage() {
    if (this.pageNumber() > 1)
      this.pageNumber.update(p => p - 1);
  }

  updateStatus(order: any, newStatus: string) {
    this.orderService.updateStatus(order.id, newStatus).subscribe({
      next: () => {
        this.toastr.success('Status updated successfully!');
        this.orders.update(list =>
          list.map(o => o.id === order.id
            ? { ...o, status: parseInt(newStatus) }
            : o
          )
        );
      },
      error: () => {
        this.toastr.error('Failed to update status!');
      }
    });
  }

  goToDetails(id: number) {
    this.router.navigate(['admin/order-details', id]);
  }
}
