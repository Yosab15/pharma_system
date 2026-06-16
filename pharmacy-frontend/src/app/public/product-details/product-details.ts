import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../core/services/cart';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  expiryDate: string;
  categoryName: string;
  imageUrl: string;
  bonusPoints: number;
  freeUnitsPerQuantity: number;
  freeUnitsCount: number;
}

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  public router = inject(Router);
  public cartService = inject(CartService);

  product = signal<Product | null>(null);
  loading = signal(true);
  selectedQuantity = signal(5);
  paymentType = signal<'cash' | 'deferred'>('cash');

  readonly quantities = [5, 10, 15, 20, 25, 30, 35, 40, 45, 50];
  Math = Math; // عشان نستخدم Math في الـ HTML
  discountPercent = 20;
  cashExtraDiscount = 3;

  get basePrice() {
    const p = this.product();
    if (!p) return 0;
    return p.price * this.selectedQuantity();
  }

  get mainDiscount() {
    return this.basePrice * (this.discountPercent / 100);
  }

  get finalPrice() {
    if (this.paymentType() === 'cash') {
      return this.basePrice - this.mainDiscount - (this.basePrice * this.cashExtraDiscount / 100);
    }
    return this.basePrice - this.mainDiscount;
  }

  // كام وحدة مجاناً بناءً على الكمية المختارة
  get freeUnits() {
    const p = this.product();
    if (!p || this.paymentType() !== 'cash') return 0;
    if (!p.freeUnitsPerQuantity || !p.freeUnitsCount) return 0;
    return Math.floor(this.selectedQuantity() / p.freeUnitsPerQuantity) * p.freeUnitsCount;
  }

  // الكمية الفعلية اللي هياخدها
  get totalUnits() {
    return this.selectedQuantity() + this.freeUnits;
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadProduct(+id);
  }

  loadProduct(id: number) {
    this.http.get<Product>(`https://localhost:44313/api/Product/${id}`)
      .subscribe({
        next: (res) => { this.product.set(res); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
  }

  getImageUrl(url: string) {
    if (!url) return '';
    return `https://localhost:44313${url}`;
  }

  addToCart() {
  const p = this.product();
  if (!p) return;

  // حفظ نوع الدفع والوحدات المجانية في الـ CartService
  this.cartService.paymentType.set(
    this.paymentType() === 'cash' ? 'Cash' : 'Deferred'
  );
  this.cartService.freeUnitsReceived.set(this.freeUnits);

  for (let i = 0; i < this.selectedQuantity(); i++) {
    this.cartService.addItem(p);
  }

  this.router.navigate(['/cart']);
}

  goBack() { this.router.navigate(['/']); }
  goToCart() { this.router.navigate(['/cart']); }
}
