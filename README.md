# TrafficSimulator

Esse projeto é um simulador inspirado no jogo Frogger, onde as condições de tráfego e clima são controladas por uma API externa.

## Características

- Tráfego dinâmico baseado na densidade de veículos via API
- Sistema meteorológico em tempo real que afeta a jogabilidade
- Sistema preditivo usando estados futuros da API
- Arquitetura orientada a eventos (sistemas desacoplados)
- Movimentação do jogador usando o Unity Input System
- Efeitos visuais (VFX) para transições climáticas
- Áudio ambiente de fundo
- Sistema de progressão de níveis com dificuldade crescente

## 🧠 Conceito Central

O jogo utiliza uma API que fornece:

- Condições atuais de tráfego e clima
- Previsões futuras com registros de data e hora

Essas previsões são enfileiradas e executadas ao longo do tempo, afetando dinamicamente:

- Taxa de geração de tráfego
- Velocidade dos veículos
- Movimento do jogador

## 📈 Progressão de Nível e Dificuldade

Cada vez que o jogador atravessa a rua com sucesso:

- O jogo avança para o próximo nível
- Uma nova requisição à API é feita imediatamente
- Um novo cenário de tráfego é carregado

### 🔥 Escala de Dificuldade

A dificuldade aumenta progressivamente a cada nível usando um multiplicador:

- Maior **densidade de veículos**
- Maior **velocidade dos veículos**
- Leve redução na **mobilidade do jogador**

> ⚠️ Importante:
> O sistema **não sobrescreve os dados da API**.

> A dificuldade é aplicada como um multiplicador sobre os valores recebidos, preservando a integridade da simulação.

## Tecnologia

- Unity (C#)
- Input System
- URP
- Couroutine-based async files
- Mock API (Mockoon)

## Como funciona

O sistema consome dados da API contendo os estados de tráfego atuais e previstos. Essas previsões são enfileiradas e aplicadas ao longo do tempo, afetando a jogabilidade de forma dinâmica.

## 🎮 Controles

- **WASD / Setas** → Mover o jogador
- O movimento é baseado em uma grade (estilo Frogger)

## 🌦️ Efeitos Climáticos

O clima impacta diretamente a jogabilidade:

| Clima | Efeito no Jogador |

|-------------|----------------|

| Ensolarado | Velocidade 100% |

| Nublado/Nebuloso | Velocidade 80% |

| Chuva Leve | Velocidade 60% |

| Chuva Forte | Velocidade 40% + Efeito Vignette |

As transições visuais são gerenciadas usando o Volume Global do Unity (Pós-processamento).

## 🔊 Áudio

- Áudio ambiente de fundo para imersão
- Projetado para aprimorar sutilmente a experiência de jogo

## ▶️ Como executar o projeto

### 1. Abra o projeto no Unity

- Versão recomendada do Unity: 6.3 LTS (6000.3.9f1)
- Abra o Unity Hub
- Clique em "Adicionar projeto"
- Selecione a pasta do projeto

---

### 2. Configure a API simulada

1. Abra o Mockoon
2. Importe o ambiente:

- Arquivo: `Assets/Project/Scripts/Mock/traffic-api-mock.json`

3. Inicie o servidor em: http://localhost:3001

---

### 3. Executar o Jogo

- Abra a cena principal em Assets/Project/Scenes/Main.unity
- Clique em **Play**

---

### 4. Fluxo do Jogo

- Clique no botão **Start**
- Atravesse a rua evitando o trânsito
- Cada travessia bem-sucedida carrega um novo cenário da API

## Author

Andre Luiz Prado de Oliveira
