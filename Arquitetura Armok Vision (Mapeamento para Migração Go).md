# Arquitetura Armok Vision (Mapeamento para Migração Go)

Este documento detalha o funcionamento interno do projeto `armok-vision-steam` (Unity) para servir de guia na migração para Go.

## 1. Comunicação e Protocolo (DFHack)

O Armok Vision atua como um cliente RPC para o plugin `RemoteFortressReader` do DFHack.

- **Conexão (`DFConnection.cs`)**: Utiliza sockets para se comunicar com o DF via Protobuf.
- **RPCs Principais**:
  - `GetBlockList`: Busca dados do mapa em blocos de 16x16x1.
  - `GetUnitList`: Lista unidades e suas posições/estados.
  - `GetMaterialList`: Definições de materiais do DF.
  - `GetTiletypeList`: Propriedades visuais e físicas dos tipos de tiles.

## 2. Estrutura de Dados do Mapa

O mapa é gerenciado em blocos para otimizar a renderização e o meshing.

- **`GameMap.cs`**: Coordenador central. Converte coordenadas do DF (X, Y, Z) para o espaço 3D.
  - No DF: X (Horizontal), Y (Vertical/Profundidade), Z (Nível/Altura).
  - No Unity: X = DF.x * 2, Y = (DF.z + Offset) * 3, Z = DF.y * -2.
- **Escalonamento**:
  - `tileHeight = 3.0f` (Eixo vertical no Unity)
  - `tileWidth = 2.0f` (Eixo horizontal no Unity)
  - `blockSize = 16`
- **`MapDataStore.cs`**: Cache local dos blocos recebidos. Gerencia a visibilidade e o estado (dirty bits) para regeneração de mesh. Implementa um sistema de "Ring Buffer" para gerenciar blocos em movimento.

## 3. Geração de Geometria (Meshing)

A parte mais complexa da migração. O projeto usa uma técnica customizada para suavizar quinas.

- **`BlockMesher.cs`**: Orquestra a criação de meshes em threads separadas.
- **`VoxelGenerator.cs`**: Implementa uma lógica de triangulação para quinas (`Diamond`, `Square`, `Rounded`).
- **Camadas de Mesh**:
  - `Base`: Terreno opaco.
  - `Cutout`: Vegetação e itens com transparência binária.
  - `Transparent`: Líquidos (água/magma).
  - `Collision`: Geometria simplificada para física.

## 4. Sistema de Materiais e Texturas (Splatting)

O visual é gerado dinamicamente usando "Splat Maps".

- **`SplatManager.cs`**: Gera texturas de controle (RG) e tint (RGBA) em tempo de execução baseadas nos materiais do DF.
- **Shaders (`Ground Splat.shader`)**:
  - Recebe um `Texture2DArray` com os padrões de materiais.
  - Faz o blend de até 4 materiais baseado em mapas de altura e texturas de controle.
  - Suporta "Contaminants" (sangue, lama) via texturas de ruído e direção.
- **Líquidos (`FlowManager.cs`)**:
  - O DF fornece níveis de fluido de 0 a 7.
  - O Mesher converte isso em altura de plano e aplica shaders de distorção de água/magma.

## 5. Entidades e Ativos

- **Criaturas (`CreatureSpriteManager.cs`)**: Carrega sprites 2D das "graphics raws" do DF e as renderiza como quads sempre voltados para a câmera (billboards).
- **Itens**: Carrega prefabs ou gera quads baseados em definições XML.
- **Vegetação**: Usa `PlantGrowthMatcher` para mapear estágios de crescimento a meshes/texturas específicos.

## 6. Lógica de Assets e Mapeamento (Content)

A ponte entre os IDs numéricos do DF e os assets 3D é feita via arquivos externos.

- **`ContentLoader.cs`**: Lê arquivos XML no diretório `StreamingAssets` que mapeiam:
  - `MaterialLayer`: Quais texturas/cores usar para cada material (pedra, madeira, etc).
  - `TileMesh`: Associa formas de tiles (paredes, rampas) a meshes `.obj`.
  - `CreatureGraphics`: Mapeia tokens de raça/profissão a páginas de tiles de sprites.
- **Raw Parsing**: O projeto possui um parser simplificado de raws do DF para extrair definições de cores e nomes (`RawLoader`).

- **Raw Parsing**: O projeto possui um parser simplificado de raws do DF para extrair definições de cores e nomes (`RawLoader`).

## 7. Sistemas Ambientais e de Tempo

O Armok Vision sincroniza o ciclo dia/noite com o tempo do jogo.

- **`DFTime.cs`**: Converte os "ticks" do DF em tempo de jogo (minutos, horas, meses).
- **Iluminação (`SunAngle.cs`)**: Calcula a posição do sol e a temperatura de cor da luz baseada na hora do dia e na estação do ano recebida do DF.

## 8. Considerações para Migração Go

Ao portar para Go (provavelmente usando um motor como `ebitengine` ou `raylib`, ou um renderizador customizado Vulkan/WGPU):

1.  **Concorrência**: Aproveitar as `goroutines` para o processo de meshing (que é intensivo em CPU no Unity).
2.  **Gerenciamento de Memória**: O Unity usa muitos buffers de mesh. Em Go, será necessário gerenciar buffers de VRAM explicitamente.
3.  **Shaders**: Os shaders HLSL precisarão ser convertidos para GLSL/MSL ou WGSL.
4.  **Protobuf**: Usar as bindings oficiais de Go para DFHack/RemoteFortressReader.

---
*Mapeamento realizado para auxiliar no projeto FortressVision.*
