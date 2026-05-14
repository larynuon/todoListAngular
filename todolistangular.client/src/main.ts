// Angular app bootstrap configuration
import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { AppComponent } from './app/app.component';

// Start the Angular application
bootstrapApplication(AppComponent, {
  providers: [
    // Enable HTTP client with fetch API for making API requests
    provideHttpClient(withFetch())
  ],
}).catch(err => console.error(err));
