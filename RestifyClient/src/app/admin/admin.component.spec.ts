import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminComponent } from './admin.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideQueryClient, QueryClient } from '@tanstack/angular-query-experimental';
import { ToastrModule } from 'ngx-toastr';
import { AdminService } from './admin.service';

describe('AdminComponent', () => {
    let component: AdminComponent;
    let fixture: ComponentFixture<AdminComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            // AdminComponent is standalone, so include it in imports
            imports: [
                AdminComponent,
                ToastrModule.forRoot()
            ],
            providers: [
                AdminService,
                provideHttpClient(),
                provideHttpClientTesting(),
                provideQueryClient(new QueryClient({
                    defaultOptions: {
                        queries: {
                            retry: false,
                            gcTime: 0,
                        },
                    },
                })),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AdminComponent);
        component = fixture.componentInstance;

        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
