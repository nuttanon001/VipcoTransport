import { OnInit, OnDestroy } from "@angular/core";
import { Subscription } from 'rxjs/Subscription';

export abstract class AbstractViewComponent<Class,Service> implements OnInit, OnDestroy {
    displayValue: Class;
    subscription: Subscription;

    constructor(
        protected service: Service,
        protected serviceCom: any,
    ) { }

    // on hook init
    ngOnInit(): void {
        this.subscription = this.serviceCom.ToChild$.subscribe(
            (displayValue: Class) => this.onGetDataByKey(displayValue));
    }

    // on get data by key
    abstract onGetDataByKey(value: Class): void;

    // on hook destory
    ngOnDestroy(): void {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}