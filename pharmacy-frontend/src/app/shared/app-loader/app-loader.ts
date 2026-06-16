import { Component } from '@angular/core';
import { LoadingService } from '../../core/services/loading';

@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [],
  templateUrl: './app-loader.html',
  styleUrl: './app-loader.css'
})
export class AppLoader {
  constructor(public loadingService: LoadingService) {}
}
