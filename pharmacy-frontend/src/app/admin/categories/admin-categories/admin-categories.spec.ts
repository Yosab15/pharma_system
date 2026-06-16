import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Admincategories } from './admin-categories';

describe('Admincategories', () => {
  let component: Admincategories;
  let fixture: ComponentFixture<Admincategories>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Admincategories],
    }).compileComponents();

    fixture = TestBed.createComponent(Admincategories);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
