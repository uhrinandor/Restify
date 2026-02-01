import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Navitem } from './navitem';
import {provideRouter, RouterLink} from '@angular/router';

describe('Navitem', () => {
  let component: Navitem;
  let fixture: ComponentFixture<Navitem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Navitem],
        providers: [provideRouter([])]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Navitem);
    component = fixture.componentInstance;
    fixture.componentRef.setInput("label", "test");
    fixture.componentRef.setInput("route", "test");
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
