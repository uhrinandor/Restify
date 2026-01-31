import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Navitem } from './navitem';

describe('Navitem', () => {
  let component: Navitem;
  let fixture: ComponentFixture<Navitem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Navitem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Navitem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
