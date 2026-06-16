import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

interface Category {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admincategories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-categories.html',
  styleUrl: './admin-categories.css'
})
export class Admincategories implements OnInit {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private baseUrl = 'https://localhost:44313/api/Category';

  categories = signal<Category[]>([]);
  loading = signal(true);
  showModal = signal(false);
  newCategoryName = signal('');

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.loading.set(true);
    this.http.get<Category[]>(this.baseUrl).subscribe({
      next: (res) => { this.categories.set(res); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  openAdd() {
    this.newCategoryName.set('');
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  save() {
    if (!this.newCategoryName()) return;
    this.http.post(this.baseUrl, { name: this.newCategoryName() }, { responseType: 'text' })
      .subscribe({
        next: () => {
          this.toastr.success('Category created successfully!');
          this.closeModal();
          this.loadCategories();
        },
        error: () => {
          this.toastr.error('Failed to create category!');
        }
      });
  }

  delete(id: number) {
    if (!confirm('Are you sure you want to delete this category?')) return;
    this.http.delete(`${this.baseUrl}/${id}`, { responseType: 'text' })
      .subscribe({
        next: () => {
          this.toastr.success('Category deleted successfully!');
          this.loadCategories();
        },
        error: (err) => {
          try {
            const body = JSON.parse(err.error);
            this.toastr.error(body.Message || 'Cannot delete this category!');
          } catch {
            this.toastr.error('Cannot delete category with existing products!');
          }
        }
      });
  }
}
