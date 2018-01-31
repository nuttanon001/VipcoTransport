export interface IScheduleWeekly {
    StartDate: Date;
    EndDate: Date;
    StartDate2: Date;
    EndDate2: Date;
    CompanyID: number;
}

export class ScheduleWeekly implements IScheduleWeekly {
    StartDate: Date;
    EndDate: Date;
    StartDate2: Date;
    EndDate2: Date;
    CompanyID: number;
}