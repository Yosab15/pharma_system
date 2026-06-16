import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Order {
  id: number;
  status: number;
  totalPrice: number;
  orderDate: string;
  customerName: string;
  city: number;
}

export interface PagedResult {
  data: Order[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export const OrderStatusMap: Record<number, string> = {
  0: 'Pending',
  1: 'Contacted',
  2: 'Delivered',
  3: 'Cancelled'
};

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:44313/api/Order';

  getOrders(page: number, pageSize: number, status?: string, search?: string, city?: string): Observable<PagedResult> {
    const statusIndexMap: Record<string, number> = {
      'Pending': 0,
      'Contacted': 1,
      'Delivered': 2,
      'Cancelled': 3
    };

    let params = new HttpParams()
      .set('PageNumber', page)
      .set('PageSize', pageSize);

    if (status) params = params.set('Status', statusIndexMap[status]);
    if (search) params = params.set('Search', search);
    if (city) params = params.set('City', city);

    return this.http.get<PagedResult>(`${this.baseUrl}/paged`, { params });
  }

  updateStatus(id: number, status: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}/status`,
      { status: parseInt(status) },
      { responseType: 'text' }
    );
  }
}
