# Proyecto Unity: Doctor Octavio persiguiendo a Spiderman

Este proyecto utiliza **Inverse Kinematics (IK)** con un algoritmo basado en gradiente para simular el movimiento dinámico y realista de los brazos mecánicos del Doctor Octavio mientras persigue a Spiderman, quien sigue una animación cíclica en un entorno 3D.

Este proyecto ha sido desarrollado por Jordi Planas Martinez, Judith Espigol Miguel y Alejandro Martinez Membrilla.

---

## Características principales

1. **Sistema de IK basado en Gradiente:**
   - Implementamos un sistema de Inverse Kinematics (IK) que utiliza un conjunto de *joints* (articulaciones) para calcular las rotaciones y posiciones necesarias, de manera que el **end effector** (la última articulación del brazo) alcance su objetivo (*target*).
   - Los ajustes se realizan dinámicamente para garantizar precisión en el movimiento.

2. **Ganchos interactivos:**
   - Los ganchos de los brazos se activan cuando el **end effector** está cerca del objetivo:
     - **Se cierran** al alcanzar el objetivo.
     - **Se abren** al alejarse.
   - Este comportamiento se logra manipulando las rotaciones locales de los dedos de los ganchos, que son hijos directos del *end effector*.

3. **Movimiento cíclico de Spiderman:**
   - Spiderman se mueve en el espacio 3D utilizando el sistema **Animator**, permitiendo observar cómo los brazos mecánicos de Octavio lo alcanzan y lo atrapan de manera precisa.

4. **Cambio de cámaras:**
   - El proyecto incluye múltiples cámaras para observar la acción desde diferentes perspectivas.
   - Puedes cambiar entre cámaras usando las teclas `1`, `2`, `3`, `4` y `5`. *(Las últimas dos cámaras son más humorísticas que funcionales 😉).*

---

## ¿Cómo funciona el sistema IK?

1. El sistema IK utiliza un **algoritmo de gradiente** para calcular las posiciones y rotaciones ideales de cada articulación (*joint*) del brazo.
2. Ajusta dinámicamente la orientación de los brazos hasta que el **end effector** alcanza el *target* definido.
3. El sistema permite simular comportamientos complejos y naturales para brazos mecánicos.

---

## Objetivo del proyecto

Demostrar el uso de Inverse Kinematics en Unity para crear animaciones dinámicas, interactivas y realistas, combinando el movimiento de un personaje con la interacción de objetos mecánicos controlados mediante algoritmos.
