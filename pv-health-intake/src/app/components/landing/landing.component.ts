import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/services/api.service';
import { LocationService } from 'src/app/services/location.service';
import { SurveyDataService } from 'src/app/services/survey-data.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {
  patientForm!: FormGroup;
  countries: any[]=[];
  states: any[]=[];
  cities: any[]=[];
  loadingStates = false;
  loadingCities = false;
  submitting = false;
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private surveyDataService: SurveyDataService,
    private locationService: LocationService,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.patientForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]+$/)]],
      dateOfBirth: ['', [Validators.required, this.pastDateValidator]],
      email: ['', [Validators.required, Validators.email]],
      country:['',Validators.required],
      state:['',Validators.required],
      city:['',Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      appointmentDate: ['', [Validators.required, this.futureDateValidator]],
      reasonForVisit: ['', Validators.required]
    });
    this.loadCountries();
    this.setupLocationListeners();
  }
  loadCountries(){
    this.locationService.getCountries().subscribe({
      next:(data)=>{
        this.countries = data;
      },
      error:(err)=>{
        console.error('Error fetching countries:', err);
      }
    });
  }
  setupLocationListeners(){
    this.patientForm.get('country')?.valueChanges.subscribe(countryCode =>{
      if(countryCode){
        this.loadingStates = true;
        this.states=[];
        this.cities=[];
        this.patientForm.patchValue({state:'',city:''});

        this.locationService.getStates(countryCode).subscribe({
          next:(data)=>{
            this.states = data;
            this.loadingStates = false;
          },
          error:(err)=>{
            console.error('Error fetching states:', err);
            this.loadingStates = false;
          }
        });
      }
    });
    this.patientForm.get('state')?.valueChanges.subscribe(stateCode=>{
      const countryCode = this.patientForm.get('country')?.value;
      if(countryCode && stateCode) {
        this.loadingCities = true;
        this.locationService.getCities(countryCode, stateCode).subscribe({
          next:(data)=>{
            this.cities = data;
            this.loadingCities = false;
          },
          error:(err)=>{
            console.error('Error fetching cities:', err);
            this.loadingCities = false;
          }
        });
      }
    })
  }

  futureDateValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    return selectedDate >= today ? null : { pastDate: true };
  }

  pastDateValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    return selectedDate < today ? null : { futureDate: true };
  }

  onSubmit() {
    if (this.patientForm.valid) {
      this.submitting = true;

      this.apiService.createPatient(this.patientForm.value).subscribe({
        next: (response) => {
          console.log('Patient created:', response);
          this.surveyDataService.setPatientData(response.data);
          this.submitting = false;
          this.router.navigate(['/survey']);
        },
        error: (err) => {
          console.error('Error creating patient:', err);
          this.submitting = false;
          alert('Failed to save patient data. Please try again.');
        }
      });
    }
  }
}
