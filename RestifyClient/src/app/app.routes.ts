import { Routes } from '@angular/router';
import {Home} from './home/home';
import {AdminComponent} from './admin/admin.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'admins', component: AdminComponent }
];
