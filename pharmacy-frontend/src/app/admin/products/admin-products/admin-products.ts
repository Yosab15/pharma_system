import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  expiryDate: string;
  categoryId: number;
  categoryName: string;
  imageUrl: string;
  bonusPoints: number;
  freeUnitsPerQuantity: number;
  freeUnitsCount: number;
}

interface Category {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.html',
  styleUrl: './admin-products.css'
})
export class AdminProducts implements OnInit {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private baseUrl = 'https://localhost:44313/api/Product';
  private categoryUrl = 'https://localhost:44313/api/Category';
  private imageBase = 'https://localhost:44313';

  products = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(true);
  showModal = signal(false);
  editMode = signal(false);
  selectedId = signal<number | null>(null);
  imagePreview = signal<string | null>(null);
  selectedFile = signal<File | null>(null);

  form = {
  name: '',
  description: '',
  price: 0,
  stockQuantity: 0,
  expiryDate: '',
  categoryId: 0,
  bonusPoints: 0,
  freeUnitsPerQuantity: 0,
  freeUnitsCount: 0,
};

  ngOnInit() {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts() {
    this.loading.set(true);
    this.http.get<Product[]>(this.baseUrl).subscribe({
      next: (res) => { this.products.set(res); this.loading.set(false); },
      error: () => {
        this.toastr.error('Failed to load products!');
        this.loading.set(false);
      }
    });
  }

  loadCategories() {
    this.http.get<Category[]>(this.categoryUrl).subscribe({
      next: (res) => this.categories.set(res),
      error: () => this.toastr.error('Failed to load categories!')
    });
  }

  getImageUrl(url: string): string {
    if (!url) return '';
    return url.startsWith('http') ? url : this.imageBase + url;
  }

 openAdd() {
  this.editMode.set(false);
  this.selectedId.set(null);
  this.imagePreview.set(null);
  this.selectedFile.set(null);
  this.form = {
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    expiryDate: '',
    categoryId: 0,
    bonusPoints: 0,
    freeUnitsPerQuantity: 0,
    freeUnitsCount: 0
  };
  this.showModal.set(true);
}

  openEdit(product: Product) {
    this.editMode.set(true);
    this.selectedId.set(product.id);
    this.imagePreview.set(this.getImageUrl(product.imageUrl));
    this.selectedFile.set(null);
    this.form = {
      name: product.name,
  description: product.description,
  price: product.price,
  stockQuantity: product.stockQuantity,
  expiryDate: product.expiryDate.split('T')[0],
  categoryId: product.categoryId,
  bonusPoints: product.bonusPoints,
  freeUnitsPerQuantity: product.freeUnitsPerQuantity,
  freeUnitsCount: product.freeUnitsCount,
    };
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile.set(file);
      const reader = new FileReader();
      reader.onload = (e: any) => this.imagePreview.set(e.target.result);
      reader.readAsDataURL(file);
    }
  }

  save() {
    const formData = new FormData();
    formData.append('name', this.form.name);
    formData.append('description', this.form.description);
    formData.append('price', this.form.price.toString());
    formData.append('stockQuantity', this.form.stockQuantity.toString());
    formData.append('expiryDate', this.form.expiryDate);
    formData.append('categoryId', this.form.categoryId.toString());
    formData.append('bonusPoints', this.form.bonusPoints.toString());
    formData.append('freeUnitsPerQuantity', this.form.freeUnitsPerQuantity.toString());
    formData.append('freeUnitsCount', this.form.freeUnitsCount.toString());
    if (this.selectedFile()) formData.append('image', this.selectedFile()!);

    if (this.editMode()) {
      this.http.put(`${this.baseUrl}/${this.selectedId()}`, formData, { responseType: 'text' })
        .subscribe({
          next: () => {
            this.toastr.success('Product updated successfully!');
            this.closeModal();
            this.loadProducts();
          },
          error: (err) => {
            try {
              const body = JSON.parse(err.error);
              this.toastr.error(body.Message || 'Failed to update product!');
            } catch {
              this.toastr.error('Failed to update product!');
            }
          }
        });
    } else {
      this.http.post(this.baseUrl, formData, { responseType: 'text' })
        .subscribe({
          next: () => {
            this.toastr.success('Product created successfully!');
            this.closeModal();
            this.loadProducts();
          },
          error: (err) => {
            try {
              const body = JSON.parse(err.error);
              this.toastr.error(body.Message || 'Failed to create product!');
            } catch {
              this.toastr.error('Failed to create product!');
            }
          }
        });
    }
  }

  delete(id: number) {
    if (!confirm('Are you sure you want to delete this product?')) return;
    this.http.delete(`${this.baseUrl}/${id}`, { responseType: 'text' })
      .subscribe({
        next: () => {
          this.toastr.success('Product deleted successfully!');
          this.loadProducts();
        },
        error: (err) => {
          try {
            const body = JSON.parse(err.error);
            this.toastr.error(body.Message || 'Failed to delete product!');
          } catch {
            this.toastr.error('Failed to delete product!');
          }
        }
      });
  }
}
