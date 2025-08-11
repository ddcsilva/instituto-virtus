import { InjectionToken } from '@angular/core';

export interface ApiConfig {
  baseUrl: string;
  version: string;
}

export const API_CONFIG = new InjectionToken<ApiConfig>('API_CONFIG');
