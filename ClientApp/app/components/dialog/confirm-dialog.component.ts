import { MdDialogRef } from '@angular/material';
import { Component } from '@angular/core';

//<md-icon color= "primary" style= "float: left; margin:-4px 5px 0px 0px;" >
//    live_help
//< /md-icon>

@Component({
    selector: 'confirm-dialog',
    template: `
    <div>
        <h4 >
        <i class="fa fa-x2 fa-meh-o" aria-hidden="true"></i>
            {{ title }}
        </h4>
    </div>
    <p>{{ message }}</p>
    <button type="submit" md-raised-button (click)="dialogRef.close(true)" color="accent">ตกลง</button>
    <button type="button" md-button (click)="dialogRef.close()" color="warn">ยกเลิก</button>
    `,
})
export class ConfirmDialog {

    public title: string;
    public message: string;

    constructor(public dialogRef: MdDialogRef<ConfirmDialog>) {

    }
}
