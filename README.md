# Encryption
Proyecto para el curso de **Estructura de Datos II**, en el cual se implementa el algoritmo de compresión de **LZW**.
Como parte del compromiso con el proyecto y el curso, se elaboraron dos soluciones, se adjunta en el link, la solución alternativa.  ([EncryptionApp](https://github.com/Ale180820/EncryptionApp.git))

## Cifrado por transposición ([Wikiwand](https://www.wikiwand.com/es/Cifrado_por_transposici%C3%B3n))
s un tipo de cifrado en el que unidades de texto plano se cambian de posición siguiendo un esquema bien definido; las 'unidades de texto' pueden ser de una sola letra (el caso más común), pares de letras, tríos de letras, mezclas de lo anterior,... Es decir, hay una permutación de 'unidades de texto'. Este tipo de algoritmos son de clave simétrica porque es necesario que tanto el que cifra como el que descifra sepan la misma clave para realizar su función.  

## Rutas y comportamiento de los métodos

#### /api/cipher/{method}
- Recibe un archivo de texto que se deberá cifrar
- Retorna un archivo del mismo nombre con el contenido del archivo cifrado. La extensión del mismo dependerá del método de cifrado utilizado:
   ○ César: .csr
   ○ ZigZag: .zz
   ○ Ruta: .rt

#### /api/decompress
- Recibe un archivo que se deberá descifrar con el método pertinente, basado en la extensión del mismo
- Retorna el archivo de texto descifrado con el mismo nombre, pero extensión txt
- Devuelve OK si no hubo error
- Devuelve InternalServerError si hubo

## Implementación
Para clonar el proyecto utilice el siguiente enlace: [https://github.com/victorisimoo/Encryption.git()]

`$ git clone https://github.com/victorisimoo/Encryption.git `
