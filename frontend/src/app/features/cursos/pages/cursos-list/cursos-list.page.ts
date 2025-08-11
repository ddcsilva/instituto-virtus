import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-cursos-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, PageHeaderComponent],
  template: `
    <app-page-header
      title="Cursos"
      subtitle="Gerencie os cursos da instituição"
    />

    <mat-card>
      <mat-card-content>
        <p>Funcionalidade em desenvolvimento...</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class CursosListPage {}
