# Exploring Multisensory Feedback for Physiotherapy Training Using XR and Robots 

> Thesis project by **Gabriella Sekar Shada** — School of Computing and Information Systems  
> Supervised by Dr. Adelaide Genay and Dr. Antony Chacon

---

## Overview

An integrated system using a **Social Robot (Furhat)** and **Mixed Reality app** to provide multisensory feedback for physiotherapy students learning spinal mobilisation techniques.

📄 **Poster**  
![Poster](poster.png)<img width="4494" height="3179" alt="Social Robot and Visualisation for Medical Training_Poster_9 Oct (1)" src="https://github.com/user-attachments/assets/54b4e3c8-9525-4293-b33b-e074b6bb6d81" />

<!-- Replace with your actual poster image path or link -->

🎥 **Demo Video**  
[![Watch the demo](https://img.shields.io/badge/Watch-Demo%20Video-red?logo=youtube)](https://youtu.be/6Exh7WmIQsI)
<!-- Replace YOUR_VIDEO_LINK_HERE with your actual video URL -->

---

## System Components

- **Pressure Sensor** — 3-layer sensor (copper rows + Eeontex fabric + copper columns)
- **ESP32** — transmits sensor data to XR headset via Bluetooth
- **Unity** — handles XR visualisation (heatmap, ghost hand, bone deformation)
- **Furhat Robot** — simulated patient with real-time voice and facial cues
