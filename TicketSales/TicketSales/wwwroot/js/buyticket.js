
document.addEventListener('DOMContentLoaded', function () {
    // HTML elementlərini seçirik
    const seatMap = document.getElementById('seatMap');
    const selectedSeatsList = document.getElementById('selectedSeatsList');
    const totalPriceElement = document.getElementById('totalPrice');
    const submitBtn = document.getElementById('submitBtn');
    const noSeatsMsg = document.getElementById('noSeats');

    // Razor ilə serverdən oturacaq məlumatları alırıq
    const ticketData = [
        @foreach(var ticket in Model.Tickets)
{
    if (ticket.TotalCount > 0) {
        <text>{type: "@ticket.SeatNumber", count: @ticket.TotalCount, price: @ticket.Price },</text>
    }
}
            ];

// Bütün oturacaqları tipinə görə array-ə yığırıq
const seatTypes = [];
ticketData.forEach(ticket => {
    for (let i = 0; i < ticket.count; i++) {
        seatTypes.push({ type: ticket.type.toLowerCase().replace(' ', ''), price: ticket.price });
    }
});

// Sətirlərin konfiqurasiyası (hər sətirdə maksimum 15 oturacaq ola bilər)
const rowsConfig = [];
const totalSeats = seatTypes.length;
const maxPerRow = 15;
let remaining = totalSeats;

while (remaining > 0) {
    const count = Math.min(remaining, maxPerRow);
    const width = 70 + Math.floor((count / maxPerRow) * 30); // Vizual uyğunluq üçün sətir genişliyi
    rowsConfig.push({ seatCount: count, width: `${width}%` });
    remaining -= count;
}

// Oturacaqları yaratmaq və HTML-ə əlavə etmək
let seatCounter = 1, rowCounter = 0;
rowsConfig.forEach((rowCfg) => {
    const row = document.createElement('div');
    row.className = 'row';
    row.style.width = rowCfg.width;

    // Sətir adı (A, B, C, ...)
    const rowLabel = document.createElement('div');
    rowLabel.className = 'row-label';
    rowLabel.textContent = String.fromCharCode(65 + rowCounter++); // A, B, C...
    row.appendChild(rowLabel);

    for (let i = 0; i < rowCfg.seatCount && seatCounter <= totalSeats; i++) {
        const seatInfo = seatTypes[seatCounter - 1];

        // Oturacaq elementi yaradılır
        const seatDiv = document.createElement('div');
        seatDiv.className = `seat ${seatInfo.type}`; // CSS tiplərinə görə stil
        seatDiv.textContent = seatCounter;
        seatDiv.dataset.seatNumber = seatCounter;
        seatDiv.dataset.price = seatInfo.price;
        seatDiv.dataset.type = seatInfo.type;

        // Form üçün checkbox (seçimlər post edilə bilsin deyə)
        const input = document.createElement('input');
        input.type = 'checkbox';
        input.name = 'SelectedSeats';
        input.value = `${seatInfo.type}-${seatCounter}`;
        input.style.display = 'none';
        seatDiv.appendChild(input);

        // Seçim toggle funksiyası
        seatDiv.addEventListener('click', toggleSeatSelection);
        row.appendChild(seatDiv);
        seatCounter++;
    }
    seatMap.appendChild(row); // Sətiri ekrana əlavə et
});

// Seçilmiş oturacaqları saxlamaq üçün xəritə (Map)
const selectedSeats = new Map();

// Oturacaq seçimi və ləğvi
function toggleSeatSelection() {
    const seatNumber = parseInt(this.dataset.seatNumber);
    const seatType = this.dataset.type;
    const seatPrice = parseFloat(this.dataset.price);
    const checkbox = this.querySelector('input[type="checkbox"]');

    if (this.classList.contains('selected')) {
        // Seçimi ləğv et
        this.classList.remove('selected');
        checkbox.checked = false;
        selectedSeats.delete(seatNumber);
        document.getElementById(`selected-seat-${seatNumber}`)?.remove();
    } else {
        // Oturacağı seç
        this.classList.add('selected');
        checkbox.checked = true;

        const rowLetter = String.fromCharCode(65 + getRowIndexBySeatNumber(seatNumber));
        selectedSeats.set(seatNumber, { type: seatType, price: seatPrice, rowLetter });

        // Seçilmiş yerlər siyahısına əlavə et
        const seatItem = document.createElement('div');
        seatItem.className = 'seat-item';
        seatItem.id = `selected-seat-${seatNumber}`;
        seatItem.innerHTML = `<span>Sıra ${rowLetter}, Yer ${seatNumber} (${seatType})</span><span>${seatPrice} AZN</span>`;
        selectedSeatsList.appendChild(seatItem);
    }

    // Əgər heç yer seçilməyibsə, mesaj göstər və button-u deaktiv et
    noSeatsMsg.style.display = selectedSeats.size === 0 ? 'block' : 'none';
    submitBtn.disabled = selectedSeats.size === 0;
    updateTotalPrice();
}

// Toplam qiyməti hesablamaq
function updateTotalPrice() {
    let total = 0;
    selectedSeats.forEach(seat => total += seat.price);
    totalPriceElement.textContent = total;
}

// Oturacağın hansı sırada olduğunu tapmaq üçün funksiya
function getRowIndexBySeatNumber(seatNumber) {
    let count = 0;
    for (let i = 0; i < rowsConfig.length; i++) {
        count += rowsConfig[i].seatCount;
        if (seatNumber <= count) return i;
    }
    return 0;
}
        
