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
