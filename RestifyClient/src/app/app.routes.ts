import { Routes } from '@angular/router';
import {Home} from './home/home';
import {Admin} from './admin/admin';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'admins', component: Admin }
];
