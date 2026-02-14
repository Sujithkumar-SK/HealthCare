# Patient Pre-Visit Health Intake Form - Flow Document

## Overview
A professional multi-step health intake form for patients to complete before their medical visit.

---

## Application Flow

### 1. LANDING PAGE (Welcome Screen)
**Purpose**: Collect basic patient information and introduce the form

**Fields**:
- Full Name (required)
- Date of Birth (required)
- Email (required)
- Phone Number (required)
- Appointment Date (required)
- Reason for Visit (text area, required)

**Actions**:
- "Start Health Assessment" button → Navigate to Survey Form

---

### 2. SURVEY FORM (Multi-Page Health Questionnaire)
**Purpose**: Comprehensive health intake using SurveyJS

#### **Page 1: Personal & Emergency Information**
- Gender (dropdown: Male, Female, Other, Prefer not to say)
- Marital Status (dropdown: Single, Married, Divorced, Widowed)
- Emergency Contact Name (required)
- Emergency Contact Phone (required)
- Emergency Contact Relationship (required)

#### **Page 2: Medical History**
- Do you have any chronic conditions? (Yes/No)
  - **IF YES**: Which conditions? (checkbox: Diabetes, Hypertension, Heart Disease, Asthma, Arthritis, Cancer, Other)
  - **IF "Other" selected**: Specify other conditions (text)
- Have you had any surgeries? (Yes/No)
  - **IF YES**: List surgeries and dates (text area)
- Do you have any allergies? (Yes/No)
  - **IF YES**: 
    - Drug Allergies (text area)
    - Food Allergies (text area)
    - Environmental Allergies (text area)

#### **Page 3: Current Medications**
- Are you currently taking any medications? (Yes/No)
  - **IF YES**: List all medications with dosage (text area)
- Are you taking any vitamins or supplements? (Yes/No)
  - **IF YES**: List vitamins/supplements (text area)

#### **Page 4: Family Medical History**
- Does anyone in your immediate family have: (checkbox)
  - Heart Disease
  - Diabetes
  - Cancer (specify type if selected)
  - High Blood Pressure
  - Mental Health Conditions
  - Other (specify)

#### **Page 5: Lifestyle & Habits**
- Do you smoke? (dropdown: Never, Former smoker, Current smoker)
  - **IF Current smoker**: Packs per day (number)
- Do you consume alcohol? (dropdown: Never, Occasionally, Regularly)
  - **IF Regularly**: Drinks per week (number)
- Exercise frequency (dropdown: None, 1-2 times/week, 3-4 times/week, 5+ times/week)
- Sleep hours per night (number: 0-12)

#### **Page 6: Current Symptoms**
- Are you experiencing any symptoms today? (Yes/No)
  - **IF YES**: Describe symptoms (text area)
  - Pain level (rating scale: 0-10)
  - Symptom duration (dropdown: Less than 1 week, 1-2 weeks, 2-4 weeks, More than 1 month)

#### **Page 7: Insurance & Consent**
- Insurance Provider (text)
- Insurance ID Number (text)
- Group Number (text)
- I consent to treatment (checkbox, required)
- I authorize release of medical information (checkbox, required)
- I agree to the privacy policy (checkbox, required)

**Navigation**:
- "Previous" button (except on first page)
- "Next" button (except on last page)
- "Review Answers" button (on last page) → Navigate to Review Page

---

### 3. REVIEW PAGE (Summary)
**Purpose**: Display all submitted information for patient verification

**Display**:
- All sections in organized cards/panels:
  - Personal Information
  - Emergency Contact
  - Medical History
  - Current Medications
  - Family History
  - Lifestyle
  - Current Symptoms
  - Insurance Information

**Actions**:
- "Edit" button → Go back to Survey Form (with data preserved)
- "Submit" button → Navigate to Confirmation Page

---

### 4. CONFIRMATION PAGE (Thank You)
**Purpose**: Confirm successful submission

**Display**:
- Success message
- Confirmation number (generated)
- Summary of appointment details
- Next steps instructions
- Contact information for questions

**Actions**:
- "Download PDF" button (optional - saves form data)
- "Start New Form" button → Navigate back to Landing Page (clears data)

---

## Conditional Logic Summary

### Show/Hide Logic:
1. **Chronic Conditions** → Show condition checkboxes only if "Yes"
2. **Other Condition** → Show text field only if "Other" is checked
3. **Surgeries** → Show text area only if "Yes"
4. **Allergies** → Show allergy details only if "Yes"
5. **Medications** → Show medication list only if "Yes"
6. **Vitamins** → Show supplement list only if "Yes"
7. **Current Smoker** → Show packs per day only if "Current smoker"
8. **Regular Alcohol** → Show drinks per week only if "Regularly"
9. **Current Symptoms** → Show symptom details only if "Yes"

### Validation Rules:
- All required fields must be filled
- Email must be valid format
- Phone must be valid format
- Date of Birth must be in the past
- Appointment Date must be in the future
- All consent checkboxes must be checked before submission

---

## Data Flow

```
Landing Page (Basic Info)
    ↓
Survey Form (Health Details) → Data stored in service
    ↓
Review Page (Verification) → Retrieve data from service
    ↓
Confirmation Page (Success) → Clear data from service
```

---

## Professional Design Elements

1. **Progress Indicator**: Show current page/total pages in survey
2. **Auto-save**: Save progress in browser localStorage
3. **Responsive Design**: Mobile-friendly layout
4. **Loading States**: Show spinner during navigation
5. **Error Handling**: Clear validation messages
6. **Accessibility**: Proper labels, ARIA attributes
7. **Professional Theme**: Clean, medical-appropriate colors (blues, whites)

---

## Technical Notes

- Use SurveyJS for the survey form (Pages 1-7)
- Use Angular Reactive Forms for landing page
- Store data in service to share between components
- Use Angular Router for navigation
- Bootstrap for responsive grid and styling
- No backend - all data stored in browser (can be extended later)

---

## Future Enhancements (Optional)
- PDF generation
- Email confirmation
- Backend API integration
- Patient portal login
- Multi-language support
- Digital signature

---

**End of Flow Document**
