import { inject } from '@angular/core';
import { Router, CanMatchFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanMatchFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.estaAutenticado()) {
      return router.parseUrl('/login');
    }

    if (authService.possuiPermissao(allowedRoles)) {
      return true;
    }

    return router.parseUrl('/');
  };
};
