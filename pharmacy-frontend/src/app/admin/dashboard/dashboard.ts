import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface DashboardStats {
  totalOrders: number;
  pendingOrders: number;
  deliveredOrders: number;
  totalRevenue: number;
  todayOrders: number;
  todayRevenue: number;
}

interface TopProduct {
  productId: number;
  productName: string;
  totalQuantity: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
   styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:44313/api/Dashboard';

  stats = signal<DashboardStats | null>(null);
  topProducts = signal<TopProduct[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.loadStats();
    this.loadTopProducts();
  }

  loadStats() {
    this.http.get<DashboardStats>(`${this.baseUrl}/stats`).subscribe({
      next: (res) => {
        this.stats.set(res);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  loadTopProducts() {
    this.http.get<TopProduct[]>(`${this.baseUrl}/top-products`).subscribe({
      next: (res) => this.topProducts.set(res)
    });
  }
}
