import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, RouterLink],
  template: `
    <div class="page-header">
      <div class="header-content">
        <h1>{{ title }}</h1>
        @if (subtitle) {
        <p class="subtitle">{{ subtitle }}</p>
        }
      </div>

      @if (actionRoute || actionClick) {
      <button
        mat-raised-button
        color="primary"
        [routerLink]="actionRoute"
        (click)="!actionRoute && actionClick && actionClick()"
      >
        @if (actionIcon) {
        <mat-icon>{{ actionIcon }}</mat-icon>
        }
        {{ actionLabel }}
      </button>
      }
    </div>
  `,
  styles: [
    `
      .page-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin: 0 0 24px 0;
        padding: 8px 0 16px 0;
        border-bottom: 1px solid #e0e0e0;
      }

      h1 {
        margin: 0 0 4px 0;
        font-size: 24px;
        font-weight: 500;
      }

      .subtitle {
        margin: 0;
        color: #666;
        font-size: 14px;
      }

      button mat-icon {
        margin-right: 8px;
      }
    `,
  ],
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle?: string;
  @Input() actionLabel?: string;
  @Input() actionIcon?: string;
  @Input() actionRoute?: string;
  @Input() actionClick?: () => void;
}
