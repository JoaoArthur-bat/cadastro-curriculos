const URL_API = "http://localhost:5195/api";

const SEM_CONEXAO = "Não foi possível conectar ao servidor. Verifique se o backend está rodando.";

// Busca a lista de candidatos
export async function listarCandidatos() {
  let resposta;
  try {
    resposta = await fetch(`${URL_API}/candidatos`);
  } catch {
    throw new Error(SEM_CONEXAO);
  }

  if (!resposta.ok) {
    throw new Error("Não foi possível carregar a lista de candidatos.");
  }
  return await resposta.json();
}

// Busca os dados de um candidato pelo número (id)
export async function buscarCandidato(id) {
  let resposta;
  try {
    resposta = await fetch(`${URL_API}/candidatos/${id}`);
  } catch {
    throw new Error(SEM_CONEXAO);
  }

  if (!resposta.ok) {
    throw new Error("Não foi possível carregar os dados do candidato.");
  }
  return await resposta.json();
}

// Salva um candidato. Devolve { sucesso, erros, mensagem }
export async function cadastrarCandidato(dados) {
  let resposta;
  try {
    resposta = await fetch(`${URL_API}/candidatos`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dados),
    });
  } catch {
    return { sucesso: false, erros: {}, mensagem: SEM_CONEXAO };
  }

  if (resposta.ok) {
    return { sucesso: true, erros: {}, mensagem: "Candidato salvo com sucesso!" };
  }

  // 400 = a validação do backend recusou algum campo
  if (resposta.status === 400) {
    const corpo = await resposta.json();
    const erros = {};
    for (const campo in corpo.errors) {
      // Guardo o nome do campo em minúsculas e só a primeira mensagem de cada um
      erros[campo.toLowerCase()] = corpo.errors[campo][0];
    }
    return { sucesso: false, erros, mensagem: "Corrija os campos destacados." };
  }

  return { sucesso: false, erros: {}, mensagem: "Erro inesperado ao salvar. Tente novamente." };
}

// Envia o PDF para o backend ler. Devolve { sucesso, dados, mensagem }
export async function extrairPdf(arquivo) {
  const formData = new FormData();
  formData.append("arquivo", arquivo);

  let resposta;
  try {
    resposta = await fetch(`${URL_API}/candidatos/extrair-pdf`, {
      method: "POST",
      body: formData,
    });
  } catch {
    return { sucesso: false, dados: null, mensagem: SEM_CONEXAO };
  }

  const corpo = await resposta.json().catch(() => ({}));

  return {
    sucesso: resposta.ok,
    dados: corpo,
    mensagem: corpo.mensagem || "Não foi possível ler o PDF.",
  };
}