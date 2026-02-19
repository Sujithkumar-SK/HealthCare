import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/services/api.service';
import { SurveyDataService } from 'src/app/services/survey-data.service';
import { Model } from 'survey-core';

@Component({
  selector: 'app-survey-form',
  templateUrl: './survey-form.component.html',
  styleUrls: ['./survey-form.component.css']
})
export class SurveyFormComponent implements OnInit {
  surveyModel!: Model;
  submitting = false;
  constructor(
    private router: Router,
    private surveyDataService: SurveyDataService,
    private apiService: ApiService
  ) { }

  ngOnInit(): void {
    const surveyJson = {
      title: "Health Assessment Questionnaire",
      showProgressBar: "top",
      progressBarType: "pages",
      pages: [
        {
          name: "personalInfo",
          title: "Personal & Emergency Information",
          elements: [
            {
              type: "dropdown",
              name: "gender",
              title: "Gender",
              isRequired: true,
              choices: ["Male", "Female", "Other", "Prefer not to say"]
            },
            {
              type: "dropdown",
              name: "maritalStatus",
              title: "Marital Status",
              isRequired: true,
              choices: ["Single", "Married", "Divorced", "Widowed"]
            },
            {
              type: "text",
              name: "emergencyContactName",
              title: "Emergency Contact Name",
              isRequired: true,
              validators: [
                {
                  type: "regex",
                  text: "Only alphabets and spaces are allowed",
                  regex: "^[a-zA-Z\s]+$"
                }
              ]
            },
            {
              type: "text",
              name: "emergencyContactRelationship",
              title: "Emergency Contact Relationship",
              isRequired: true
            },
            {
              type: "text",
              name: "emergencyContactPhone",
              title: "Emergency Contact Phone",
              isRequired: true,
              inputType: "tel",
              validators: [
                {
                  type: "regex",
                  text: "Phone number must be exactly 10 digits",
                  regex: "^[0-9]{10}$"
                }
              ]
            }
          ]
        },
        {
          name: "medicalHistory",
          title: "Medical History",
          elements: [
            {
              type: "boolean",
              name: "hasChronicConditions",
              title: "Do you have any chronic conditions?",
              isRequired: true
            },
            {
              type: "checkbox",
              name: "chronicConditions",
              title: "Which conditions?",
              visibleIf: "{hasChronicConditions} = true",
              isRequired: true,
              choices: ["Diabetes", "Hypertension", "Heart Disease", "Asthma", "Arthritis", "Cancer", "Other"]
            },
            {
              type: "text",
              name: "otherChronicCondition",
              title: "Please specify other condition",
              visibleIf: "{chronicConditions} contains 'Other'"
            },
            {
              type: "boolean",
              name: "hadSurgeries",
              title: "Have you had any surgeries?",
              isRequired: true
            },
            {
              type: "comment",
              name: "surgeriesList",
              title: "List surgeries and dates",
              visibleIf: "{hadSurgeries} = true",
              isRequired: true
            },
            {
              type: "boolean",
              name: "hasAllergies",
              title: "Do you have any allergies?",
              isRequired: true
            },
            {
              type: "comment",
              name: "drugAllergies",
              title: "Drug Allergies",
              visibleIf: "{hasAllergies} = true"
            },
            {
              type: "comment",
              name: "foodAllergies",
              title: "Food Allergies",
              visibleIf: "{hasAllergies} = true"
            },
            {
              type: "comment",
              name: "environmentalAllergies",
              title: "Environmental Allergies",
              visibleIf: "{hasAllergies} = true"
            }
          ]
        },
        {
          name: "medications",
          title: "Current Medications",
          elements: [
            {
              type: "boolean",
              name: "takingMedications",
              title: "Are you currently taking any medications?",
              isRequired: true
            },
            {
              type: "comment",
              name: "medicationsList",
              title: "List all medications with dosage",
              visibleIf: "{takingMedications} = true",
              isRequired: true
            },
            {
              type: "boolean",
              name: "takingSupplements",
              title: "Are you taking any vitamins or supplements?",
              isRequired: true
            },
            {
              type: "comment",
              name: "supplementsList",
              title: "List vitamins/supplements",
              visibleIf: "{takingSupplements} = true",
              isRequired: true
            }
          ]
        },
        {
          name: "familyHistory",
          title: "Family Medical History",
          elements: [
            {
              type: "checkbox",
              name: "familyConditions",
              title: "Does anyone in your immediate family have:",
              choices: ["Heart Disease", "Diabetes", "Cancer", "High Blood Pressure", "Mental Health Conditions", "Other"]
            },
            {
              type: "text",
              name: "cancerType",
              title: "Please specify type of cancer",
              visibleIf: "{familyConditions} contains 'Cancer'"
            },
            {
              type: "text",
              name: "otherFamilyCondition",
              title: "Please specify other condition",
              visibleIf: "{familyConditions} contains 'Other'"
            }
          ]
        },
        {
          name: "lifestyle",
          title: "Lifestyle & Habits",
          elements: [
            {
              type: "dropdown",
              name: "smokingStatus",
              title: "Do you smoke?",
              isRequired: true,
              choices: ["Never", "Former smoker", "Current smoker"]
            },
            {
              type: "text",
              name: "packsPerDay",
              title: "Packs per day",
              visibleIf: "{smokingStatus} = 'Current smoker'",
              inputType: "number"
            },
            {
              type: "dropdown",
              name: "alcoholConsumption",
              title: "Do you consume alcohol?",
              isRequired: true,
              choices: ["Never", "Occasionally", "Regularly"]
            },
            {
              type: "text",
              name: "drinksPerWeek",
              title: "Drinks per week",
              visibleIf: "{alcoholConsumption} = 'Regularly'",
              inputType: "number"
            },
            {
              type: "dropdown",
              name: "exerciseFrequency",
              title: "Exercise frequency",
              isRequired: true,
              choices: ["None", "1-2 times/week", "3-4 times/week", "5+ times/week"]
            },
            {
              type: "text",
              name: "sleepHours",
              title: "Sleep hours per night",
              isRequired: true,
              inputType: "number",
              min: 0,
              max: 12
            }
          ]
        },
        {
          name: "symptoms",
          title: "Current Symptoms",
          elements: [
            {
              type: "boolean",
              name: "hasSymptoms",
              title: "Are you experiencing any symptoms today?",
              isRequired: true
            },
            {
              type: "comment",
              name: "symptomsDescription",
              title: "Describe symptoms",
              visibleIf: "{hasSymptoms} = true",
              isRequired: true
            },
            {
              type: "rating",
              name: "painLevel",
              title: "Pain level (0 = No pain, 10 = Worst pain)",
              visibleIf: "{hasSymptoms} = true",
              rateMin: 0,
              rateMax: 10
            },
            {
              type: "dropdown",
              name: "symptomDuration",
              title: "Symptom duration",
              visibleIf: "{hasSymptoms} = true",
              choices: ["Less than 1 week", "1-2 weeks", "2-4 weeks", "More than 1 month"]
            }
          ]
        },
        {
          name: "insurance",
          title: "Insurance & Consent",
          elements: [
            {
              type: "text",
              name: "insuranceProvider",
              title: "Insurance Provider",
              isRequired: true
            },
            {
              type: "text",
              name: "insuranceId",
              title: "Insurance ID Number",
              isRequired: true
            },
            {
              type: "text",
              name: "groupNumber",
              title: "Group Number"
            },
            {
              type: "boolean",
              name: "consentTreatment",
              title: "I consent to treatment",
              isRequired: true
            },
            {
              type: "boolean",
              name: "consentMedicalInfo",
              title: "I authorize release of medical information",
              isRequired: true
            },
            {
              type: "boolean",
              name: "consentPrivacy",
              title: "I agree to the privacy policy",
              isRequired: true
            }
          ]
        },
        {
          name: "documents",
          title: "Upload Documents",
          elements: [
            {
              type: "file",
              name: "medicalRecords",
              title: "Upload Medical Records (Optional)",
              description: "You can upload previous medical records, test results, or prescriptions (PDF, JPG, PNG)",
              allowMultiple: true,
              maxSize: 5242880,
              acceptedTypes: ".pdf,.jpg,.jpeg,.png"
            },
            {
              type: "file",
              name: "insuranceCard",
              title: "Upload Insurance Card (Optional)",
              description: "Upload front and back of your insurance card",
              allowMultiple: true,
              maxSize: 5242880,
              acceptedTypes: ".pdf,.jpg,.jpeg,.png"
            }
          ]
        }
      ],
      completeText: "Review Answers",
      showQuestionNumbers: "off"
    };
    this.surveyModel = new Model(surveyJson);

    this.surveyModel.onComplete.add((sender) => {
      this.submitting = true;
      const patientData = this.surveyDataService.getPatientData();
      console.log('Patient data from service:', patientData);

      const surveyPayload = {
        patientId: patientData.id,
        surveyData: sender.data
      };

      this.apiService.createSurvey(surveyPayload).subscribe({
        next: (response) => {
          console.log('Survey created:', response);
          this.surveyDataService.setSurveyData(response.data);
          this.submitting = false;
          this.router.navigate(['/review']);
        },
        error: (err) => {
          console.error('Error creating survey:', err);
          this.submitting = false;
          alert('Failed to save survey. Please try again.');
        }
      });
    });
  }

  
}
