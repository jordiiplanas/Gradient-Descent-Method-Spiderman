Nuestro proyecto tiene inputs que permiten cambiar de camara para poder observar desde diferentes 
perspectivas el movimiento del personaje.
Cambias entre ellas con el 1,2,3,4,5 (las ultimas dos son más de broma)

---------

Para la entrega en el codigo implementamos un sistema de inverseIK haciendo uso de un algoritmo gradient
, que usa como "brazos" un conjunto de joints que nosotros le pasamos por referencia. El objetivo
es ajustar las rotaciones y posiciones de los objetos para que el end effector (joint final del brazo)
alcance alcance al target.

Para los ganchos hemos calculado la distancia del endeffector al target para cerrarlo cuando se encuentra cercano
y abrirlo cuando se abre, todo esto aplicado rotaciones locales a los dedos del gancho que son hijos del endfactor

Para el movimiento del spiderman hemos usado el animator para hacer el spiderman moverse por el espacio 3D de manera
que se pueda OBSERVAR como los brazos lo alcanzan y lo cogen.

