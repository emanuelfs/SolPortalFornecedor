function cpfValido(value) {
    cleanCpf = value.replace(/[^\d]+/g, '');

    if (isNullOrEmpty(cleanCpf)) {
        return true;
    }

    if (cleanCpf.length != 11 ||
        cleanCpf == "00000000000" ||
        cleanCpf == "11111111111" ||
        cleanCpf == "22222222222" ||
        cleanCpf == "33333333333" ||
        cleanCpf == "44444444444" ||
        cleanCpf == "55555555555" ||
        cleanCpf == "66666666666" ||
        cleanCpf == "77777777777" ||
        cleanCpf == "88888888888" ||
        cleanCpf == "99999999999") {
        return false;
    }

    add = 0;
    for (i = 0; i < 9; i++) {
        add += parseInt(cleanCpf.charAt(i)) * (10 - i);
    }
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11) {
        rev = 0;
    }
    if (rev != parseInt(cleanCpf.charAt(9))) {
        return false;
    }

    add = 0;
    for (i = 0; i < 10; i++) {
        add += parseInt(cleanCpf.charAt(i)) * (11 - i);
    }
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11) {
        rev = 0;
    }
    if (rev != parseInt(cleanCpf.charAt(10))) {
        return false;
    }
    return true;
}

function cnpjValido(value) {
    cleanCnpj = value.replace(/[^\d]+/g, '');

    if (isNullOrEmpty(cleanCnpj)) {
        return true;
    }

    if (cleanCnpj.length != 14) {
        return false;
    }

    if (cleanCnpj == "00000000000000" ||
        cleanCnpj == "11111111111111" ||
        cleanCnpj == "22222222222222" ||
        cleanCnpj == "33333333333333" ||
        cleanCnpj == "44444444444444" ||
        cleanCnpj == "55555555555555" ||
        cleanCnpj == "66666666666666" ||
        cleanCnpj == "77777777777777" ||
        cleanCnpj == "88888888888888" ||
        cleanCnpj == "99999999999999") {
        return false;
    }

    tamanho = cleanCnpj.length - 2
    numeros = cleanCnpj.substring(0, tamanho);
    digitos = cleanCnpj.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2) {
            pos = 9;
        }
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0)) {
        return false;
    }

    tamanho = tamanho + 1;
    numeros = cleanCnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2) {
            pos = 9;
        }
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(1)) {
        return false;
    }

    return true;
}