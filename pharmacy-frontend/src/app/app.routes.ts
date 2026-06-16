import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { cartGuard } from './core/guards/cart-guard';

export const routes: Routes = [

  // ================= PUBLIC =================
  {
    path: '',
    loadComponent: () =>
      import('./public/products/products')
        .then(m => m.Products)
  },

  {
    path: 'product/:id',
    loadComponent: () =>
      import('./public/product-details/product-details')
        .then(m => m.ProductDetails)
  },

  {
    path: 'order',
    canActivate: [cartGuard],
    loadComponent: () =>
      import('./public/create-order/create-order')
        .then(m => m.CreateOrder)
  },

  {
    path: 'cart',
    loadComponent: () =>
      import('./public/cart/cart')
        .then(m => m.Cart)
  },

  // ================= ADMIN LOGIN =================
  {
    path: 'admin/login',
    loadComponent: () =>
      import('./admin/login/login')
        .then(m => m.Login)
  },

  // ================= ADMIN LAYOUT =================
  {
    path: 'admin',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./admin/layout/admin-layout/admin-layout')
        .then(m => m.AdminLayout),

    children: [
      {
        path: '',
        loadComponent: () =>
          import('./admin/dashboard/dashboard')
            .then(m => m.Dashboard)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./admin/orders/orders')
            .then(m => m.Orders)
      },
      {
        path: 'order-details/:id',
        loadComponent: () =>
          import('./admin/order-details/order-details')
            .then(m => m.OrderDetails)
      },
      {
        path: 'product',
        loadComponent: () =>
          import('./admin/products/admin-products/admin-products')
            .then(m => m.AdminProducts)
      },
      {
        path: 'category',
        loadComponent: () =>
          import('./admin/categories/admin-categories/admin-categories')
            .then(m => m.Admincategories)
      }
    ]
  },

  // ================= NOT FOUND =================
  {
    path: '**',
    loadComponent: () =>
      import('./public/not-found/not-found')
        .then(m => m.NotFound)
  },
];
