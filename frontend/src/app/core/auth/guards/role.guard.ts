import { inject } from '@angular/core';
import { Router, CanMatchFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanMatchFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated()) {
      return router.parseUrl('/login');
    }

    if (authService.hasRole(allowedRoles)) {
      return true;
    }

    return router.parseUrl('/');
  };
};
