window.onload = () => {

     const slider = document.getElementById("ecSlider");
     let pos = 0;
     const move = 255 * 4;

     const max = slider.scrollWidth - slider.parentElement.clientWidth;

     document.querySelector(".ec-next").onclick = () => {
          pos = Math.min(pos + move, max);
          slider.style.transform = `translateX(-${pos}px)`;
     };

     document.querySelector(".ec-prev").onclick = () => {
          pos = Math.max(pos - move, 0);
          slider.style.transform = `translateX(-${pos}px)`;
     };

};