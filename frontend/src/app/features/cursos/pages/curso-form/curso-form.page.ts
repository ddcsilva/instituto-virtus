import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-curso-form',
  standalone: true,
  imports: [CommonModule, MatCardModule, PageHeaderComponent],
  template: `
    <app-page-header
      title="Formulário de Curso"
      subtitle="Cadastre ou edite um curso"
    />

    <mat-card>
      <mat-card-content>
        <p>Funcionalidade em desenvolvimento...</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class CursoFormPage {}
