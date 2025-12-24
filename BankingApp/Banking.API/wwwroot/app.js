const API_URL = '/api/accounts';
let currentAccount = null;
let currentAction = null;

// UI Helpers
const get = (id) => document.getElementById(id);
const show = (id) => get(id).classList.remove('hidden');
const hide = (id) => get(id).classList.add('hidden');

function notify(message, isError = false) {
    const notif = get('notification');
    notif.textContent = message;
    notif.style.borderColor = isError ? 'var(--danger-color)' : 'var(--success-color)';
    notif.style.color = isError ? '#fecaca' : '#d1fae5';
    notif.classList.add('show');
    setTimeout(() => notif.classList.remove('show'), 3000);
}

// Auth Logic
async function createAccount() {
    const owner = get('ownerName').value;
    const type = get('accountType').value;

    if (!owner) return notify('Please enter a name', true);

    try {
        const res = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ owner, accountType: type })
        });
        
        if (!res.ok) throw new Error(await res.text());
        
        const account = await res.json();
        loginSuccess(account);
        notify(`Welcome, ${owner}! Account created.`);
    } catch (err) {
        notify(err.message, true);
    }
}

async function login() {
    const accNum = get('loginAccountNumber').value;
    if (!accNum) return notify('Enter Account Number', true);

    try {
        const res = await fetch(`${API_URL}/${accNum}`);
        if (!res.ok) throw new Error('Account not found');
        
        const account = await res.json();
        loginSuccess(account);
    } catch (err) {
        notify(err.message, true);
    }
}

function loginSuccess(account) {
    currentAccount = account;
    hide('auth-view');
    show('dashboard-view');
    updateDashboard();
}

function logout() {
    currentAccount = null;
    hide('dashboard-view');
    show('auth-view');
    get('ownerName').value = '';
    get('loginAccountNumber').value = '';
}

function updateDashboard() {
    if (!currentAccount) return;
    
    get('displayBalance').textContent = `$${currentAccount.balance.toFixed(2)}`;
    get('displayAccountInfo').textContent = `${currentAccount.owner} • ${currentAccount.accountNumber} • ${accountTypeToString(currentAccount.type)}`;
    
    const list = get('transactionList');
    list.innerHTML = '';
    
    // Reverse to show newest first
    const txs = [...currentAccount.transactions].reverse();
    
    if (txs.length === 0) {
        list.innerHTML = '<div style="text-align:center; padding: 20px; color: #64748b;">No transactions yet</div>';
    }

    txs.forEach(tx => {
        const el = document.createElement('div');
        el.className = 'transaction-item';
        const isPos = tx.amount > 0;
        el.innerHTML = `
            <div>
                <div style="font-weight:600">${tx.description}</div>
                <div style="font-size:0.8rem; color:#94a3b8">${new Date(tx.date).toLocaleDateString()}</div>
            </div>
            <div class="${isPos ? 'amount-positive' : 'amount-negative'}">
                ${isPos ? '+' : ''}$${Math.abs(tx.amount).toFixed(2)}
            </div>
        `;
        list.appendChild(el);
    });
}

function accountTypeToString(type) {
    return type === 0 ? 'Savings' : 'Checking';
}

// Modal Logic
function showModal(action) {
    currentAction = action;
    get('modalTitle').textContent = action.charAt(0).toUpperCase() + action.slice(1);
    get('modalAmount').value = '';
    get('modalDestAccount').value = '';
    
    if (action === 'transfer') {
        show('destAccountGroup');
    } else {
        hide('destAccountGroup');
    }

    const modal = get('actionModal');
    modal.style.opacity = '1';
    modal.style.pointerEvents = 'all';
}

function hideModal() {
    const modal = get('actionModal');
    modal.style.opacity = '0';
    modal.style.pointerEvents = 'none';
}

async function executeAction() {
    const amount = parseFloat(get('modalAmount').value);
    if (!amount || amount <= 0) return notify('Invalid amount', true);

    try {
        let endpoint = '';
        let body = {};

        if (currentAction === 'deposit') {
            endpoint = '/deposit';
            body = { accountNumber: currentAccount.accountNumber, amount };
        } else if (currentAction === 'withdraw') {
            endpoint = '/withdraw';
            body = { accountNumber: currentAccount.accountNumber, amount };
        } else if (currentAction === 'transfer') {
            const dest = get('modalDestAccount').value;
            if (!dest) return notify('Enter destination account', true);
            endpoint = '/transfer';
            body = { 
                sourceAccountNumber: currentAccount.accountNumber,
                destinationAccountNumber: dest,
                amount 
            };
        }

        const res = await fetch(`${API_URL}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        if (!res.ok) throw new Error(await res.text());

        // Refresh account data
        // For transfer, we might get a message, so best to re-fetch the account
        const refreshRes = await fetch(`${API_URL}/${currentAccount.accountNumber}`);
        currentAccount = await refreshRes.json();
        
        updateDashboard();
        hideModal();
        notify('Transaction successful!');
    } catch (err) {
        notify(err.message, true);
    }
}
