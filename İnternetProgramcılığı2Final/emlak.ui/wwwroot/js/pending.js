let currentPropertyId = null;

function loadPendingProperties() {
    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/pending`,
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                const properties = response.data;
                const tbody = $('#pendingPropertiesTable tbody');
                tbody.empty();

                properties.forEach(property => {
                    // Lokasyon bilgilerini kontrol et
                    const cityName = property.city?.name || '';
                    const districtName = property.district?.name || '';
                    const locationText = cityName && districtName ? `${cityName} / ${districtName}` : '';

                    // İlan sahibi bilgilerini kontrol et
                    const ownerName = property.user ? 
                        `${property.user.firstName || ''} ${property.user.lastName || ''}`.trim() : 
                        '';

                    // Tarih formatı
                    const createdAt = new Date(property.createdAt).toLocaleDateString('tr-TR');

                    tbody.append(`
                        <tr>
                            <td>${property.title || ''}</td>
                            <td>${property.price ? property.price.toLocaleString('tr-TR') : '0'} ₺</td>
                            <td>${locationText}</td>
                            <td>${ownerName}</td>
                            <td>${createdAt}</td>
                            <td>
                                <button class="btn btn-sm btn-info" onclick="viewProperty(${property.id})">
                                    <i class="fas fa-eye"></i>
                                </button>
                            </td>
                        </tr>
                    `);
                });
            }
        },
        error: function(xhr) {
            console.error('Onay bekleyen ilanlar yüklenirken hata oluştu:', xhr);
            toastr.error('Onay bekleyen ilanlar yüklenirken bir hata oluştu');
        }
    });
}

function viewProperty(id) {
    currentPropertyId = id;
    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/${id}`,
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                const property = response.data;
                
                // İlan bilgilerini doldur
                $('#modalTitle').text(property.title);
                $('#modalPrice').text(property.price.toLocaleString('tr-TR') + ' ₺');
                $('#modalSquareMeters').text(property.squareMeters + ' m²');
                $('#modalRoomCount').text(property.roomCount + ' + 1');
                $('#modalBathroomCount').text(property.bathroomCount);
                $('#modalPropertyType').text(property.propertyType);
                $('#modalDescription').text(property.description);
                $('#modalAddress').text(property.address);
                $('#modalCity').text(property.city?.name || '');
                $('#modalDistrict').text(property.district?.name || '');

                // Özellikleri doldur
                const featuresDiv = $('#modalFeatures');
                featuresDiv.empty();
                if (property.features && property.features.length > 0) {
                    property.features.forEach(feature => {
                        featuresDiv.append(`
                            <span class="badge bg-primary">${feature.name}: ${feature.value}</span>
                        `);
                    });
                }

                // Resimleri doldur
                const imagesDiv = $('#modalImages');
                imagesDiv.empty();
                if (property.images && property.images.length > 0) {
                    property.images.forEach(image => {
                        const fullImageUrl = image.imageUrl.startsWith('/') 
                            ? `${config.apiUrl}${image.imageUrl}`
                            : image.imageUrl;
                            
                        imagesDiv.append(`
                            <img src="${fullImageUrl}" class="img-thumbnail" style="width: 150px; height: 150px; object-fit: cover;">
                        `);
                    });
                }

                $('#propertyModal').modal('show');
            }
        },
        error: function(xhr) {
            console.error('İlan detayları alınırken hata oluştu:', xhr);
            toastr.error('İlan detayları alınırken bir hata oluştu');
        }
    });
}

function approveProperty() {
    if (!currentPropertyId) return;

    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/${currentPropertyId}/approve`,
        type: 'PUT',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                $('#propertyModal').modal('hide');
                loadPendingProperties();
                toastr.success('İlan başarıyla onaylandı');
            }
        },
        error: function(xhr) {
            console.error('İlan onaylanırken hata oluştu:', xhr);
            toastr.error('İlan onaylanırken bir hata oluştu');
        }
    });
}

function rejectProperty() {
    if (!currentPropertyId) return;

    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/${currentPropertyId}/reject`,
        type: 'PUT',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                $('#propertyModal').modal('hide');
                loadPendingProperties();
                toastr.success('İlan başarıyla reddedildi');
            }
        },
        error: function(xhr) {
            console.error('İlan reddedilirken hata oluştu:', xhr);
            toastr.error('İlan reddedilirken bir hata oluştu');
        }
    });
}

// Sayfa yüklendiğinde onay bekleyen ilanları yükle
$(document).ready(function() {
    loadPendingProperties();
});

$document.read(function () {
    console.log("zortinger");
})
   
