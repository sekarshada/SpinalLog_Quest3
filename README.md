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

Back pain is the world's leading cause of disability, yet physiotherapy training still relies 
heavily on peer practice and verbal feedback — lacking the precision needed to develop fine 
tactile control and clinical empathy.

This project presents a **Mixed Reality training system** for spinal mobilisation that combines:
- 🔴 **Pressure sensing** — visualised as heatmaps, force graphs, and bone deformation overlays
- 🤖 **Social robot (Furhat)** — simulates a real patient with affective facial and voice responses
- 🖐️ **Ghost hand guidance** — instructs correct hand technique in MR

The study shows that immersive training systems can maintain _**stable cognitive load**_ across different feedback configurations, indicating good usability and integration. While all feedback types were perceived as clear and helpful, _**presenting too many cues simultaneously increased cognitive effort**_, highlighting the importance of timing and simplicity in interface design. The inclusion of a socially expressive robot enhanced engagement and encouraged _**patient-centred awareness**_, supporting empathy without adding mental load.  
Additionally, participants expressed a strong preference for **adaptive feedback systems**—valuing multiple feedback types, but only when presented selectively rather than all at once.
  

## 🔍 Research Problem  

Physiotherapy training, especially spinal mobilisation, relies heavily on peer practice and verbal instruction, which lacks:
- precise feedback on applied force  
- visibility into internal body response  
- patient-like interaction for empathy development
- safety during peers manual skill training 

While prior research explores feedback modalities individually, **their combined effect in immersive learning environments remains underexplored**.

---

## ✨ Research Contributions  

This work contributes across multiple dimensions:

**Empirical:** Provides evidence on how **instructional guidance and multimodal feedback** (visual + robotic) influence cognitive load, task clarity and user engagement  

**Theoretical:** Proposes a conceptual understanding of how preview guidance, real-time visualisation, embodied robotic feedback may help to shape perception and learning in MR environments  

**Methodological:** Introduces a **mixed-method evaluation framework** combining workload metrics, structured questionnaires, qualitative reflection and designed for **early-stage XR learning system evaluation**

**Artefact:** Develops a novel integrated platform combining Mixed Reality (Unity + Meta Quest 3), Real-time pressure sensing (ESP32) and Social robotics (Furhat) for affective feedback  

**Design Insights:** Provides actionable guidelines for when to use preview guidance vs real-time feedback, how social cues influence engagement, designing immersive healthcare training systems  

---

## 🧪 Methodology  

- **Study Design:** Within-subject, task-based experiment  
- **Approach:** Mixed-method (quantitative + qualitative)  
- **Focus:** Interaction between guidance and feedback modalities  

**Outcome Measures:**
- perceived workload  
- task clarity and understanding  
- user engagement  

## Participatory Design Insights  

To explore future directions beyond the current prototype, participants were invited to sketch their preferred system configurations after completing the study.  

These user-generated designs provided insight into how learners envision ideal training environments, revealing preferences that extend beyond the constraints of the implemented system.  

Across sketches, participants consistently expressed a desire for more adaptive, personalised feedback, often combining visual, embodied, and social cues. Many proposals also suggested increased realism and responsiveness, indicating the importance of both technical precision and experiential engagement in training design.  

These findings complement the experimental results by uncovering latent user needs and informing future directions for immersive healthcare training systems.

## Thesis  

📎 [Read Full Thesis (PDF)](https://bit.ly/GabriellaThesis)

<img width="5056" height="3576" alt="GITHUB" src="https://github.com/user-attachments/assets/a9cfe196-c276-4b0d-a6c0-ec343fca83a9" />

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

---

## Author  

**Gabriella Sekar Shada**  
XR / HCI Researcher with 5+ years of software engineering experience  
Interested in immersive visualisation, human-centered systems, and AI-assisted education  
