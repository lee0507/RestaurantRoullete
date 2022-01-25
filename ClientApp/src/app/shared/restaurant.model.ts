export class Restaurant {
  constructor(public id: number, public text: string, public fillStyle?: string) {
    this.id = id;
    this.text = text;
    this.fillStyle = fillStyle;
  }
}
