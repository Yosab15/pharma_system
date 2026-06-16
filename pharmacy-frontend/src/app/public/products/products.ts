import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CartService } from '../../core/services/cart';
import { LoadingService } from '../../core/services/loading';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products implements OnInit {
  private loadingService = inject(LoadingService);

  allProducts = signal<any[]>([]);
  categories = signal<any[]>([]);
  selectedCategoryId = signal<number | null>(null);
  searchTerm = signal('');

  products = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const catId = this.selectedCategoryId();
    let list = this.allProducts();

    if (catId) {
      list = list.filter(p => p.categoryId === catId);
    }

    if (term) {
      list = list.filter(p =>
        p.name.toLowerCase().includes(term) ||
        p.description?.toLowerCase().includes(term)
      );
    }

    return list;
  });

  constructor(
    private http: HttpClient,
    public router: Router,
    public cartService: CartService
  ) {}

  ngOnInit(): void {
    this.getProducts();
    this.getCategories();
  }

  getProducts() {
    this.http.get<any[]>('https://localhost:44313/api/Product')
      .subscribe({
        next: (res) => {
          this.allProducts.set(res);
          this.loadingService.hide();
        },
        error: (err) => {
          console.log(err);
          this.loadingService.hide();
        }
      });
  }

  getCategories() {
    this.http.get<any[]>('https://localhost:44313/api/Category')
      .subscribe({
        next: (res) => this.categories.set(res),
        error: (err) => console.log(err)
      });
  }

  filterByCategory(categoryId: number) {
    this.selectedCategoryId.set(categoryId);
    this.searchTerm.set('');
  }

  showAllProducts() {
    this.selectedCategoryId.set(null);
    this.searchTerm.set('');
  }

  getImage(imageUrl: string) {
    if (!imageUrl) return 'https://via.placeholder.com/300x300';
    return `https://localhost:44313${imageUrl}`;
  }


  goToCart() {
    this.router.navigate(['/cart']);
  }

  goToAdmin() {
    this.router.navigate(['/admin/login']);
  }

  goToDetails(id: number) {
  this.router.navigate(['/product', id]);
}
}
