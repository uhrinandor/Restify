import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import {provideHttpClient, withFetch, withInterceptors} from '@angular/common/http';
import { apiInterceptor } from '../interceptors/api-interceptor';
import { provideTanStackQuery, QueryClient } from '@tanstack/angular-query-experimental';
import {provideToastr} from 'ngx-toastr';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), provideClientHydration(withEventReplay()),
      provideHttpClient(withFetch(), withInterceptors([apiInterceptor])),
      provideTanStackQuery(new QueryClient()),
      provideToastr({
          timeOut: 3000,
          positionClass: 'toast-bottom-right',
      })
  ]
};
