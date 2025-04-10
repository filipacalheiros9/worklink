const response = await fetch('/api/PerfilApi/GetMeuPerfil');
const data = await response.json();
document.getElementById('idUtilizador').value = data.idUtilizador;