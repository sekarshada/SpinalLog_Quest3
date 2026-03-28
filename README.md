# Designing Multisensory Feedback for Physiotherapy Training Using XR and Robots 

> Thesis project by **Gabriella Sekar Shada** — School of Computing and Information Systems
> 🏆 Grade: H1 — 87/100

> 🥇 Top 5 Project — Research, Innovation and Commercialisation Awards, Endeavour Exhibition, University of Melbourne (2025)

> 📱 Featured in a student research showcase by the University of Melbourne — [Watch on TikTok](https://vt.tiktok.com/ZSuAX6GQf/)

> Supervised by Dr. Adelaide Genay and Dr. Antony Chacon

![Platform](https://img.shields.io/badge/Platform-Mixed%20Reality-blueviolet?logo=unity)
![Social Robot](https://img.shields.io/badge/Robot-Furhat-orange)
![Sensor](https://img.shields.io/badge/Sensor-ESP32-red?logo=espressif)
![Domain](https://img.shields.io/badge/Domain-Physiotherapy-teal)
![XR](https://img.shields.io/badge/XR-Extended%20Reality-blue)
![Unity](https://img.shields.io/badge/Engine-Unity-black?logo=unity)
![Bluetooth](https://img.shields.io/badge/Comms-Bluetooth-0082FC?logo=bluetooth)
![OptiTrack](https://img.shields.io/badge/Motion%20Capture-OptiTrack-black)
![Motive](https://img.shields.io/badge/Software-Motive-blue)
![Manus](https://img.shields.io/badge/Gloves-Manus%20Quantum%20Metagloves-purple)

---

## Overview

An integrated system using a **Social Robot (Furhat)** and **Mixed Reality app** to provide multisensory feedback for physiotherapy students learning spinal mobilisation techniques.

<img width="4494" height="3179" alt="Social Robot and Visualisation for Medical Training_Poster_9 Oct (1)" src="https://github.com/user-attachments/assets/54b4e3c8-9525-4293-b33b-e074b6bb6d81" />

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


## Hardware & Setup

### Circuit Design
<img width="422" height="154" alt="circuit (1)" src="https://github.com/user-attachments/assets/ef847f91-19c2-4804-bad4-f39ccb35b5c8" />

*ESP32 v2 connected to dual 16-channel analog multiplexers for pressure sensor data acquisition*

### Furhat Robot — Simulated Patient Expressions
<img width="395" height="197" alt="Furhat expression (1)" src="https://github.com/user-attachments/assets/87ce39e9-397c-44d4-8755-89d71938e901" />

*Furhat robot facial expressions: (a) neutral, (b) pain response — providing real-time emotional feedback during training*

### Motion Capture Setup — OptiTrack
<img width="800" height="450" alt="spatial data comparison data usability empathy assesment (6)" src="https://github.com/user-attachments/assets/55331cea-1dbe-4462-9841-2913bd87f015" />

*5-camera OptiTrack system with Motive software used to capture hand instruction movements via Manus XR gloves*
