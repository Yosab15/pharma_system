import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../core/services/cart';
import * as L from 'leaflet';

delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'assets/marker-icon-2x.png',
  iconUrl: 'assets/marker-icon.png',
  shadowUrl: 'assets/marker-shadow.png',
});

@Component({
  selector: 'app-create-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-order.html',
  styleUrl: './create-order.css',
})
export class CreateOrder {

  constructor(
    public cartService: CartService,
    private http: HttpClient,
    private router: Router
  ) {}

  readonly cities = [
    { value: 0, label: 'Cairo' },
    { value: 1, label: 'Alexandria' },
    { value: 2, label: 'Giza' },
    { value: 3, label: 'Shubra El Kheima' },
    { value: 4, label: 'Port Said' },
    { value: 5, label: 'Suez' },
    { value: 6, label: 'Luxor' },
    { value: 7, label: 'Mansoura' },
    { value: 8, label: 'El Mahalla El Kubra' },
    { value: 9, label: 'Tanta' },
    { value: 10, label: 'Asyut' },
    { value: 11, label: 'Ismailia' },
    { value: 12, label: 'Faiyum' },
    { value: 13, label: 'Zagazig' },
    { value: 14, label: 'Aswan' },
    { value: 15, label: 'Damietta' },
    { value: 16, label: 'Damanhur' },
    { value: 17, label: 'Minya' },
    { value: 18, label: 'Beni Suef' },
    { value: 19, label: 'Qena' },
    { value: 20, label: 'Sohag' },
    { value: 21, label: 'Hurghada' },
    { value: 22, label: 'Sixth of October' },
    { value: 23, label: 'Shibin El Kom' },
    { value: 24, label: 'Banha' },
    { value: 25, label: 'Kafr El Sheikh' },
    { value: 26, label: 'Arish' },
    { value: 27, label: 'Mallawi' },
    { value: 28, label: 'Mit Ghamr' },
    { value: 29, label: 'Obour' },
  ];

  form = {
    customerName: '',
    responsibleName: '',
    phoneNumber: '',
    address: '',
    city: null as number | null,
    notes: '',
  };

  submitting = false;
  success = false;
  errorMsg = '';

  cityDropdownOpen = false;
  citySearch = '';
  filteredCities = [...this.cities];

  supplyOrderFile: File | null = null;
  supplyOrderPreview: string | null = null;

  showMap = false;
  map: L.Map | null = null;
  marker: L.Marker | null = null;
  latitude: number | null = null;
  longitude: number | null = null;
  locationName = '';

  onCitySearch(term: string) {
    this.filteredCities = this.cities.filter(c =>
      c.label.toLowerCase().includes(term.toLowerCase())
    );
    this.cityDropdownOpen = true;
  }

  selectCity(city: { value: number; label: string }) {
    this.form.city = city.value;
    this.citySearch = city.label;
    this.cityDropdownOpen = false;
    this.filteredCities = [...this.cities];
  }

  onSupplyOrderChange(event: any) {
    const file = event.target.files?.[0];
    if (!file) return;
    this.supplyOrderFile = file;
    const reader = new FileReader();
    reader.onload = (e: any) => this.supplyOrderPreview = e.target.result;
    reader.readAsDataURL(file);
  }

  openMap() {
    this.showMap = true;
    setTimeout(() => {
      this.initMap();
      setTimeout(() => this.map?.invalidateSize(), 300);
    }, 50);
  }

  closeMap() {
    this.showMap = false;
  }

  getCurrentLocation() {
    if (!navigator.geolocation) return;
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        const lat = pos.coords.latitude;
        const lng = pos.coords.longitude;
        this.latitude = lat;
        this.longitude = lng;
        this.locationName = `${lat.toFixed(4)}, ${lng.toFixed(4)}`;
        this.map?.setView([lat, lng], 15);
        if (this.marker) this.map?.removeLayer(this.marker);
        this.marker = L.marker([lat, lng])
          .addTo(this.map!)
          .bindPopup('Your Location')
          .openPopup();
      },
      () => alert('Could not get your location')
    );
  }

  confirmLocation() {
    if (this.latitude !== null && this.longitude !== null)
      this.locationName = `${this.latitude.toFixed(4)}, ${this.longitude.toFixed(4)}`;
    this.closeMap();
  }

  initMap() {
    if (typeof window === 'undefined') return;
    if (this.map) { this.map.remove(); this.map = null; }

    setTimeout(() => {
      this.map = L.map('map', { center: [30.0444, 31.2357], zoom: 10, zoomControl: true });

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap', maxZoom: 19
      }).addTo(this.map);

      this.map.on('click', (e: L.LeafletMouseEvent) => {
        this.latitude = e.latlng.lat;
        this.longitude = e.latlng.lng;
        this.locationName = `${this.latitude.toFixed(4)}, ${this.longitude.toFixed(4)}`;
        if (this.marker) this.map?.removeLayer(this.marker);
        this.marker = L.marker([this.latitude, this.longitude])
          .addTo(this.map!)
          .bindPopup('Selected Location')
          .openPopup();
      });

      this.map.invalidateSize();
    }, 200);
  }

  canSubmit() {
    return !!(
      this.form.customerName.trim() &&
      this.form.responsibleName.trim() &&
      this.form.phoneNumber.trim() &&
      this.form.address.trim() &&
      this.form.city !== null &&
      this.supplyOrderFile !== null &&
      this.cartService.items().length > 0
    );
  }

  submit() {
    if (!this.canSubmit()) return;

    this.submitting = true;
    this.errorMsg = '';

    const formData = new FormData();
    formData.append('customerName', this.form.customerName.trim());
    formData.append('responsibleName', this.form.responsibleName.trim());
    formData.append('phoneNumber', this.form.phoneNumber.trim());
    formData.append('address', this.form.address.trim());
    formData.append('city', String(this.form.city));
    formData.append('notes', this.form.notes?.trim() || '');
    formData.append('paymentType', this.cartService.paymentType());
    formData.append('freeUnitsReceived', String(this.cartService.freeUnitsReceived()));

    if (this.supplyOrderFile)
      formData.append('supplyOrderImage', this.supplyOrderFile);

    if (this.latitude !== null && this.longitude !== null) {
      formData.append('latitude', String(this.latitude));
      formData.append('longitude', String(this.longitude));
    }

    this.cartService.items().forEach((item, index) => {
      formData.append(`items[${index}].productId`, String(item.productId));
      formData.append(`items[${index}].quantity`, String(item.quantity));
    });

    this.http.post<any>('https://localhost:44313/api/Order', formData)
      .subscribe({
        next: () => {
          this.submitting = false;
          this.success = true;
          this.cartService.clear();
        },
        error: (err) => {
          this.submitting = false;
          this.errorMsg =
            err?.error?.message ||
            err?.error?.title ||
            (typeof err?.error === 'string' ? err.error : '') ||
            'Request failed';
        }
      });
  }

  goBack() { this.router.navigate(['/cart']); }
  goHome() { this.router.navigate(['/']); }
}
