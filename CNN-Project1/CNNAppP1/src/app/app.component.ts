import { Component } from '@angular/core';
import * as tf from '@tensorflow/tfjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'CNNApp1';
  ipData: any;
  file: any;
  url: string | ArrayBuffer;
  image: HTMLImageElement;

  flowers = ['Black-grass','Charlock','Cleavers','Common Chickweed','Common wheat','Fat Hen','Loose Silky-bent','Maize','Scentless Mayweed','Shepherds Purse','Small-flowered Cranesbill','Sugar beet']
  predictedFlower : string;
  async func(myForm) {
    var modelUrl = 'https://raw.githubusercontent.com/nandhukumar86/AIML-Practice/master/CNN-Project1/tfmodelCNNP1.json';
    var model = await tf.loadLayersModel(modelUrl);
    var image = document.getElementById("img") as HTMLImageElement;
    var tensorImage = tf.browser.fromPixels(image).resizeNearestNeighbor([128, 128]).div(tf.scalar(255)).toFloat().expandDims();
    var output = model.predict(tensorImage) as tf.Tensor;
    var outputflowerId = this.argMax(output.dataSync());
    this.predictedFlower = this.flowers[outputflowerId];
  }

  fileChanged(event) {
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]);

      reader.onload = (event) => {
        this.url = event.target.result;
      }
    } 
  }

  argMax(array) {
    return [].map.call(array, (x, i) => [x, i]).reduce((r, a) => (a[0] > r[0] ? a : r))[1];
  }
}