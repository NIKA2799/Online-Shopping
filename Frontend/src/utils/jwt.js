// decodeToken: safely parse the payload of a JWT
export function decodeToken(token) {
  try {
    // split header.payload.signature
    const payload = token.split(".")[1];
    // atob handles standard base64; replace URL‐safe chars:
    const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(json);
  } catch (e) {
    console.error("Failed to decode token:", e);
    return null;
  }
}
