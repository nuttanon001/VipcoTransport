import { Component } from '@angular/core';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {

    onOpenNewLink(link: string): void {
        if (link)
            window.open(link, '_blank');
    }
}
