import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../auth/services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'Ocorreu um erro inesperado';

      if (error.status === 401) {
        message = 'Sessão expirada. Faça login novamente.';
        authService.logout();
      } else if (error.status === 403) {
        message = 'Você não tem permissão para realizar esta ação';
      } else if (error.status === 404) {
        message = 'Recurso não encontrado';
      } else if (error.status === 422) {
        message = error.error?.message || 'Dados inválidos';
      } else if (error.status >= 500) {
        message = 'Erro no servidor. Tente novamente mais tarde.';
      }

      snackBar.open(message, 'Fechar', {
        duration: 5000,
        horizontalPosition: 'end',
        verticalPosition: 'top',
        panelClass: ['error-snackbar'],
      });

      return throwError(() => error);
    })
  );
};
