export const getNearstDay = (day: string | Date, days: Date[]): Date =>{
    if (days.length === 0){
        return new Date();
    }

    var targetDate = typeof day === "string" ? new Date(day) : day;
    let min = Math.abs(targetDate.getTime() - days[0].getTime());
    let locIndex = 0;
    for (let i = 0; i < days.length; i++){
        let diff = Math.abs(targetDate.getTime() - days[i].getTime());
        if (targetDate.getFullYear() === days[i].getFullYear() && targetDate.getMonth() === days[i].getMonth() && targetDate.getDate() === days[i].getDate()){
            return days[i];
        }

        if (min > diff){
            min = diff;
            locIndex = i;
        }
    }

    return days[locIndex];
};