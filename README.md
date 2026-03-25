# TrafficSimulator

Esse projeto é um simulador inspirado no jogo Frogger, onde as condições de tráfego e clima são controladas por uma API externa.

## Features

- Tráfego dinâmico baseado na densidade de veículos
- Clima impacta a gameplay
- Sistema de previsões utilizando API data
- Arquitetura orientada a eventos
- Sistema de Input moderno da Unity
- Transições suaves e efeitos

## Tech

- Unity (C#)
- Input System
- URP
- Couroutine-based async files
- Mock API (Mockonn)

## Mock API Setup

1. Abra o Mockoon
2. Importe enviroment: - File: `Assets/Project/Scripts/Mock/traffic-api-mock.json`
3. Inicie o servidor
4. Endpoint default: http://localhost:3001/v1/traffic/status

## Como funciona

O sistema consome dados da API contendo os estados de tráfego atuais e previstos. Essas previsões são enfileiradas e aplicadas ao longo do tempo, afetando a jogabilidade de forma dinâmica.

## Controles

- WASD / Arrow Keys

## Author

Andre Luiz Prado de Oliveira
