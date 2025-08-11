import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function telefoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;

    const telefone = control.value.replace(/\D/g, '');

    if (telefone.length < 10 || telefone.length > 11) {
      return { telefone: 'Telefone inválido' };
    }

    return null;
  };
}
