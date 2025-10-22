# Milestone 04
## Informações Gerais
Entrega: 22/10/2025 <br>
Status: Concluído <br>
Integrantes:
- Daniel Barroquelo
- Gabriel Mezet
- Giovana Couto
- Júlia Velozo
- Pedro Terezzino
- Yuri Telis <br>

## Proposta
- Testes de gameplay, ajustes de design (2x a 3x ciclos) <br>

Para esta milestone, nos foi solicitado que fizessemos grupos de testes e fosse anotados problemas na _gameplay_. Desta forma, neste documento terão os problemas e suas correções 

## Testes e Correção de Bugs
Na tabela abaixo estão os _reports_ de bugs, junto do status e sua correção (se tiver):

| _BUG_ | _STATUS_ | _MOTIVO_ | _CORREÇÃO_ |
| :-- | :-- | :-- | :--
| Botôes da UI estão com problema para serem pressionados | Corrigido | Era um problema de hierarquia, o Panel estava atrapalhando a interação, embora em testes anteriores isso não ter atrapalhado | Bastava arrastar o Panel para o local certo da hierarquia |
| A barra de sanidade do jogador não baixava quando estava entre 7 e 8, ela "pulava" valores | Corrigido | a | a |
| A animação da Julie (protagonista) não estava fluída e muito rápida | Corrigido |  |  |
| Boss atravessando o cenário no _leap attack_ | Corrigido | Falta de colisão em certos pontos do cenário | Ajustar a colisão corretamente |
| Dash da julie ao estar muito proxima de um inimigo, faz com que ela empurre o inimigo ao invés de atravessá-lo | Corrigido | A colisão da julie com inimigos da layer Enemy não é desativada no dash, ela só fica imune a dano | Desativar colisão da mesma com inimigos na respectiva layer |
| O botão de interação com a interface de viagem no tempo precisava ser pressionado 2x para interagir com ela na primeira vez | Corrigido | código habilitava somente a interface e não os botões simultaneamente, o que fazia com que a interação do jogador com a engine fosse: ao apertar E a primeira vez a interface ativava (embora não ficasse visível pro player), e na segunda com a interface já ativa, os botões eram ativados. além disso, o bug ainda pode ocorrer caso inicialize o teste deixando a própria interface desativada na unity. | Ajustar o código para que não fosse necessário essa interação com clique duplo |
| Ao usar o stealth attack em um inimigo próximo à parede, a parede some junto | Não Corrigido | O possível motivo é a interação do inimigo com a layer de colisão, que todos os objetos estão na mesma layer para os inimigos não verem a Julie | - |
| Ataque básico do boss não reproduz animação e só causa dano na primeira iteração | Não Corrigido | - | - |
| ⁠telegraph do leap attack do boss ultrapassa o player mesmo a lógica do código tentando fazer com que isso não aconteça, causando uma representação visual que não condiz com o alcance real do golpe | Não Corrigido | - | - |
| - | - | - | - |
