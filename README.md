# MyProductAPI
Esta Api cadastra produtos atravez do codigo de barra do produto. 
Existe um cadastro de usuario de outra API que usei para colocar um token para validar o acesso a api 
para que pessoaspessoas não autorizadas não consigam usar.
1 Foram usados os pacotes do EntityFrameworkCore para persistir os dados em um banco de dados SQL Server.
2 FluentValidation para validar os dados de entrada.
3 Swagger para documentar a API.	
4 JWT Bearer para validar o token de acesso.	
5 AutoMapper para mapear os dados de entrada e saída.
6 fluentmigrations para criar as migrations do banco de dados.
7 A parte de exceptions foi criada para padronizar as mensagens de erro e facilitar a manutenção do código.	
8 obs:- o banco esta apontado para o SQLServer LocalDB, caso queira usar outro banco de dados, 
  altere a string de conexão no arquivo appsettings.json. 
9 tanto os useCase e os repositories foram criados para separar a lógica de negócio da lógica de acesso a dados, 
  facilitando a manutenção do código e a testabilidade.
   