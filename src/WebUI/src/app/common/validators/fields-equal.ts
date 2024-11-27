import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function fieldsEqual(fieldOne: string, fieldTwo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        let controlOne = control.get(fieldOne);
        let controlTwo = control.get(fieldTwo);
        if(controlOne && controlTwo && controlOne.value !== controlTwo.value) {
            let errorText = fieldOne + " doesn't match " + fieldTwo;
            return { mismatch: true, details: errorText }
        }
        return null;
    }
}